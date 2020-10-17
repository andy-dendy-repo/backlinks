using Backlinks_LE.Models;
using Backlinks_LE.Models.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backlinks_LE.Services.Extract
{
    public class FileExtract : ExtractBase, IExtract
    {
        public bool ContainsExtraInfo { get; set; }
        public FileExtract(DataBacklinkRowService service, DataListedInfoService infoService, Settings settings)
        {
            _serv = service;
            _infoServ = infoService;
            _settings = settings;
        }
        public async Task<int> Extract()
        {
            List<string> list = File.ReadAllLines(_settings.OpenDataString).ToList();
            int i = 0;

            int blin = _settings.BacklinkIndex;
            int dlin = _settings.DomainIndex;
            string del = _settings.Delimeter;
            string dom = _settings.SingleDomname;

            if (dom != null)
                dlin = -1;
            foreach (var x in list)
            {
                try
                {
                    var arr = x.Split(new string[] { del }, StringSplitOptions.None);
                    string rw=arr[blin].Replace("\"", "");
                    string domname = dom == null ? arr[dlin] : dom;
                    if (!string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(rw))
                    {
                        DataBacklinkRow data = new DataBacklinkRow() { Domain = domname, Url = rw, Position = i };
                        
                        await _serv.CreateDataBacklinkRow(data);
                        if(ContainsExtraInfo)
                        {
                            DataListedInfo info = new DataListedInfo() { DataBacklinkRowID = i, Text = Newtonsoft.Json.JsonConvert.SerializeObject(arr.Where((x, j) => j != dlin && j != blin)) };

                            await _infoServ.CreateDataListedInfo(info);
                        }

                        i++;
                    }
                }
                catch
                {

                }
            }
            return i;
        }
    }
}
