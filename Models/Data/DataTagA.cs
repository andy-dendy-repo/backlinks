using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backlinks_LE.Models.Data
{
    public class DataTagA
    {
        [Key]
        public int ID { get; set; }
        public string Href { get; set; }
        public string Rel { get; set; }

        public string Anchor { get; set; }
        public int DataBacklinkRowID { get; set; }

        public List<string> Info { get => new List<string> { Href, Rel, Anchor }; }
    }
}
