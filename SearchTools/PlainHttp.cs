using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Backlinks_LE.Models;

namespace Backlinks_LE.SearchTools
{
    public class PlainHttp : SearchBase
    {
        private static HttpClient _client;
        public PlainHttp(int threads, FreeThread handler) : base(threads, handler)
        {
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromSeconds(60);
        }

        protected override async Task<string> GetResp(string url)
        {
            try
            {
                HttpResponseMessage resp = await _client.GetAsync(url);
                string respT = await resp.Content.ReadAsStringAsync();
                return respT;
            }
            catch
            {

            }
            return null;
        }
    }
}
