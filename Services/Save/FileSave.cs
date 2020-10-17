using Backlinks_LE.Models;
using Backlinks_LE.Models.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Backlinks_LE.Services.Save
{
    public class FileSave : SaveBase, ISave
    {
        public FileSave(
            DataBacklinkRowService service,
            DataListedInfoService infoService,
            DataTagAService tagServ,
            Settings settings):base(service, infoService,tagServ,settings)
        {
            
        }

        public async Task Save()
        {
            int pos = _settings.Positions;
            string save = _settings.SaveDataString;
            for (int i = 0; i < pos; i++)
            {
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


                    await File.AppendAllTextAsync(save, string.Join("   ",line) + "\r\n");

                }
            }
        }
    }
}
