using Backlinks_LE.Models;
using Backlinks_LE.Models.Data;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backlinks_LE.Services.Save
{
    public class ExcelSave : SaveBase, ISave
    {
        public ExcelSave(
            DataBacklinkRowService service,
            DataListedInfoService infoService,
            DataTagAService tagServ,
            Settings settings)
            : base(service, infoService, tagServ, settings)
        {

        }
        public async Task Save()
        {
            int pos = _settings.Positions;

            Application ex = new Application();
            
            ex.Visible = true;
            
            ex.SheetsInNewWorkbook = 1;
            
            Workbook workBook = ex.Workbooks.Add(Type.Missing);
            
            ex.DisplayAlerts = false;
            
            Worksheet sheet = (Worksheet)ex.Worksheets.get_Item(1);
            
            sheet.Name = "Test";

            //Пример заполнения ячеек
            for (int i = 1; i <= pos; i++)
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


                    

                    int len = line.Count;


                    for (int j = 1; j <= len; j++)
                    {
                        sheet.Cells[i+1, j] = line[j-1];
                    }
                }
            }

            ex.Application.ActiveWorkbook.SaveAs(
                _settings.SaveDataString, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing
            );

            

        }
    }
}
