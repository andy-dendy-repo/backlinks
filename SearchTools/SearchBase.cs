using Backlinks_LE.Models;
using Backlinks_LE.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backlinks_LE.SearchTools
{
    public abstract class SearchBase
    {
        public delegate Task<BacklinkRow> FreeThread(BacklinkRow row);
        public FreeThread FreeThreadHandler;
        protected int _threads;
        public SearchBase(int threads, FreeThread handler)
        {
            FreeThreadHandler = handler;
            _threads = threads;
        }

        public async Task Perform(List<BacklinkRow> list)
        {
            List<Task> Tlist = new List<Task>();
            for (int i = 0; i < _threads; i++)
                Tlist.Add(Find(list[i]));

            await Task.WhenAll(Tlist);
        }
        protected async Task Find(BacklinkRow row)
        {
            if (row == null || row.DataBacklinkRow == null)
                return;

            string url = row.DataBacklinkRow.Url;

            string respT =await GetResp(url);

            row.RespText = respT;

            await Find(await FreeThreadHandler(row));
        }

        protected abstract Task<string> GetResp(string url);
    }
}
