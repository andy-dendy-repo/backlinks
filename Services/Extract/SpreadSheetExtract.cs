using Backlinks_LE.Models;
using Backlinks_LE.Models.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Backlinks_LE.Services.Extract
{
    public class SpreadSheetExtract : ExtractBase, IExtract
    {
        public SpreadSheetExtract(DataBacklinkRowService service, DataListedInfoService infoService, Settings settings)
        {
            _serv = service;
            _infoServ = infoService;
            _settings = settings;
        }

        public async Task<int> Extract()
        {
            int blin = _settings.BacklinkIndex;
            int dlin = _settings.DomainIndex;
            string dom = _settings.SingleDomname;

            if (dom != null)
                dlin = -1;

            SheetsService service;
            string sheet = "Sheet1";
            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            string ApplicationName = "ValidateLinks";

            GoogleCredential credential;
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });



            var range = $"{sheet}!A:G";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(_settings.OpenDataString, range);

            var response = request.Execute();
            IList<IList<object>> values = response.Values;

            var dt = new DataTable();
            if (values != null && values.Count > 0)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    IList<object> row = values[i];
                    List<object> f = row.AsEnumerable().ToList<object>();

                    for (int j = 0; j < f.Count; j++)
                    {
                        if (i == 0) { dt.Columns.Add(f[j].ToString()); }
                        if (i != 0)
                        {
                            if (j == 0)
                            {
                                DataRow dr = dt.NewRow();
                                dr[j] = f[j];
                                dt.Rows.Add(dr);
                            }
                            else
                            {
                                try
                                {
                                    DataRow dr = dt.Rows[i - 1];
                                    dr[j] = f[j];
                                }
                                catch
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Error...");
            }
            dt.TableName = "links";

            int len = dt.Rows.Count;
            int clen = dt.Columns.Count;
            int counter = 0;
            for (int i = 0; i < len; i++)
            {
                if (!string.IsNullOrEmpty(dt.Rows[i][dlin].ToString()) && !string.IsNullOrEmpty(dt.Rows[i][blin].ToString()) && dt.Rows[i][blin].ToString() != "n/a")
                {
                    var arr = dt.Rows[i].ItemArray;

                    DataBacklinkRow data = new DataBacklinkRow() { Domain = dt.Rows[i][blin].ToString(), Url = dt.Rows[i][dlin].ToString(), Position = counter };

                    DataListedInfo info = new DataListedInfo() { DataBacklinkRowID = counter, Text = Newtonsoft.Json.JsonConvert.SerializeObject(arr.Where((x, j) => j != dlin && j != blin)) };

                    await _infoServ.CreateDataListedInfo(info);

                    await _serv.CreateDataBacklinkRow(data);

                    counter++;
                }
            }
            service.Dispose();
            return counter;
        }
    }
}
