using Backlinks_LE.Models;
using Backlinks_LE.Models.Data;
using Backlinks_LE.SearchTools;
using Backlinks_LE.Services;
using Backlinks_LE.Services.Extract;
using Backlinks_LE.Services.Save;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Backlinks_LE
{
    public class SearchManager
    {
        private WebDriver _webDriver;
        private PlainHttp _plainHttp;
        private Settings _settings;
        private Status CurrentTaskStatus = Status.InAQueue;
        private DbQueue _dbQueue;
        private DataBacklinkRowService _dataBacklinkRowService;
        private DataListedInfoService _dataListedInfoService;
        private DataTagAService _dataTagAService;

        public SearchManager(Settings settings)
        {
            _dataBacklinkRowService = new DataBacklinkRowService(settings.ConnectionString);
            _dataListedInfoService = new DataListedInfoService(settings.ConnectionString);
            _dataTagAService = new DataTagAService(settings.ConnectionString);
            _settings = settings;
        }
        
        public async Task<int> Open(Action<string> info)
        {
            using (MyDbContext _context = new MyDbContext(_settings.ConnectionString))
            {
                _context.Database.EnsureDeleted();
                _context.Database.EnsureCreated();
            }
            IExtract extract=null;
            switch (_settings.ExtractMethod)
            {
                case DataMethods.File:
                    {
                        var extract_C = new FileExtract(_dataBacklinkRowService, _dataListedInfoService, _settings) { ContainsExtraInfo = false };
                        break;
                    }
                case DataMethods.GoogleDocs:
                    {
                        extract = new SpreadSheetExtract(_dataBacklinkRowService, _dataListedInfoService, _settings);
                        break;
                    }
            }

            int i=await extract.Extract();

            _settings.Positions = i;
            _dbQueue = new DbQueue(_settings.ConnectionString, _settings.PlainHttpThreads, i);
            DbQueue.Info = info;

            return i;
        }
        public async Task Close()
        {
            ISave save = null;
            switch (_settings.ExtractMethod)
            {
                case DataMethods.File:
                    {
                        save = new FileSave(_dataBacklinkRowService, _dataListedInfoService, _dataTagAService, _settings);
                        
                        break;
                    }
                case DataMethods.GoogleDocs:
                    {
                        save = new SpreadSheetSave(_dataBacklinkRowService, _dataListedInfoService,_dataTagAService, _settings);
                        break;
                    }
                case DataMethods.Excell:
                    {
                        save = new ExcelSave(_dataBacklinkRowService, _dataListedInfoService, _dataTagAService, _settings);
                        break;
                    }
            }

            await save.Save();
        }
        public async Task Perform()
        {

            List<BacklinkRow> list = new List<BacklinkRow>();

            for (int i = 0; i < _settings.PlainHttpThreads; i++)
            {
                DataBacklinkRow datarow = await _dataBacklinkRowService.GetFirstDataBacklinkRowByStatus(CurrentTaskStatus);
                datarow.Status = Status.PlainHttp;
                await _dataBacklinkRowService.UpdateDataBacklinkRow(datarow);
                list.Add(new BacklinkRow() { DataBacklinkRow = datarow });
            }


            _plainHttp = new PlainHttp(_settings.PlainHttpThreads, FreeThread);
            await _plainHttp.Perform(list);

            list.Clear();
            CurrentTaskStatus = Status.PlainHttpNotFound;
            _dbQueue = new DbQueue(_settings.ConnectionString, _settings.PlainHttpThreads, _settings.Positions);
            for (int i = 0; i < _settings.WebDriverThreads; i++)
            {
                DataBacklinkRow datarow = await _dataBacklinkRowService.GetFirstDataBacklinkRowByStatus(CurrentTaskStatus);
                if(datarow==null)
                {
                    _settings.WebDriverThreads = i;
                    break;
                }
                datarow.Status = Status.WebDriver;
                await _dataBacklinkRowService.UpdateDataBacklinkRow(datarow);
                list.Add(new BacklinkRow() { DataBacklinkRow = datarow });
            }
            _webDriver = new WebDriver(_settings.WebDriverThreads, FreeThread);
            await _webDriver.Perform(list);

            _webDriver.Quit();
        }
        private async Task<BacklinkRow> FreeThread(BacklinkRow row)
        {
            DataBacklinkRowService serv1 = new DataBacklinkRowService(_settings.ConnectionString);
            DataTagAService serv2 = new DataTagAService(_settings.ConnectionString);

            await serv1.UpdateDataBacklinkRow(row.DataBacklinkRow);

            if(row.DataTagA!=null)
                await serv2.CreateDataTagA(row.DataTagA);

            DataBacklinkRow datarow = _dbQueue.Get();

            BacklinkRow newrow = new BacklinkRow() { DataBacklinkRow = datarow };
            return newrow;
        }
        class DbQueue
        {
            public static Action<string> Info;
            public Status CurrentTaskStatus = Status.InAQueue;
            public int _threads,_current=0, _positions;
            public int Current { get => _current; }
            private DataBacklinkRowService _serv;
            private ConcurrentQueue<DataBacklinkRow> _queue=new ConcurrentQueue<DataBacklinkRow>();
            private object _locker = new object();
            public DbQueue(string ConnectionString, int threads, int postions)
            {
                _serv = new DataBacklinkRowService(ConnectionString);
                _threads = threads;
                _positions = postions;
                AddObjects();
            }
            private void AddObjects()
            {
                lock (_locker)
                {
                    try
                    {
                        while (_current < _positions && _queue.Count < _threads * 2)
                        {
                            
                            DataBacklinkRow data = _serv.GetFirstDataBacklinkRowByStatusSync(CurrentTaskStatus);
                            if (data == null)
                                return;
                            _queue.Enqueue(data);
                            data.Status = data.Status == Status.InAQueue ? Status.PlainHttp : Status.WebDriver;
                            _serv.UpdateDataBacklinkRowSync(data);
                            //task2.Start();
                            //task2.Wait();
                            _current++;
                            Info($"{_current}");
                        }
                    }
                    catch
                    {

                    }
                }
            }
            private void Update()
            {
                if (_queue.Count < _threads)
                {
                    Thread thread = new Thread(AddObjects);
                    thread.Start();
                }
            }
            public DataBacklinkRow Get()
            {
                DataBacklinkRow row;
                _queue.TryDequeue(out row);

                Update();

                return row;
            }
        }
    }
}
