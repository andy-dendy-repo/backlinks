using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backlinks_LE.Models.Data
{
    public class DataBacklinkRow
    {
        [Key]
        public int ID { get; set; }
        public string Domain { get; set; }
        public string Url { get; set; }
        public int Position { get; set; }
        public Status Status { get; set; }

        public List<string> Info { get => new List<string> { Domain, Url }; }

    }
}
