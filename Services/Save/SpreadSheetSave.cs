using Backlinks_LE.Models;
using Backlinks_LE.Models.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backlinks_LE.Services.Save
{
    public class SpreadSheetSave : SaveBase, ISave
    {
        public SpreadSheetSave(
            DataBacklinkRowService service,
            DataListedInfoService infoService,
            DataTagAService tagServ,
            Settings settings)
            :base(service, infoService, tagServ, settings)
        {

        }

        public async Task Save()
        {
            int pos = _settings.Positions;

            SheetsService service;
            string sheet = "sheet1";
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

            var range = $"{sheet}!A:ZZ";
            var valueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
            var meta = new List<IList<object>>() { };


            for (int i = 0; i < pos; i++)
            {
                //string res = "";
                DataBacklinkRow row;
                DataTagA tagA;
                DataListedInfo info;

                List<string> line = new List<string>();
                row = await _serv.GetDataBacklinkRowByID(i);

                if (row != null)
                {
                    line.AddRange(row.Info);

                    tagA = await _tagServ.GetDataTagAForDataBacklinkRow(row.ID);

                    if (tagA != null)
                        line.AddRange(tagA.Info);
                    else
                        line.AddRange(new string[] { "", "", "" });

                    info = await _infoServ.GetDataListedInfoByDataBacklinkRowID(row.ID);

                    if (info != null)
                        line.AddRange(Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(info.Text));


                    meta.Add(line.Select(x => x as object).ToList());
                }
            }

            valueRange.Values = meta;


            var appendRequest = service.Spreadsheets.Values.Update(valueRange, _settings.SaveDataString, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = appendRequest.Execute();
        }
    }
}
