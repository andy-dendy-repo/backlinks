using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backlinks_LE.Models.Data
{
    public class DataListedInfo
    {
        [Key]
        public int ID { get; set; }
        public string Text { get; set; }
        public int DataBacklinkRowID { get; set; }

        public List<string> Info { get => new List<string> { Text }; }

    }
}
