using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Backlinks_LE.Models
{
    public enum DataMethods
    {
        File,
        GoogleDocs,
        Excell
    }
    public class Settings
    {
        public DataMethods ExtractMethod { get; set; }
        public DataMethods SaveMethod { get; set; }
        public int Positions { get; set; }
        public string OpenDataString { get; set; }
        public string SaveDataString { get; set; }
        public int PlainHttpThreads { get; set; }
        public int WebDriverThreads { get; set; }
        public string ConnectionString { get; set; }
        public string SingleDomname { get; set; }
        public string Delimeter { get; set; }
        public int BacklinkIndex { get; set; }
        public int DomainIndex { get; set; }


        public void Save(string path="settings.json")
        {
            string text = JsonConvert.SerializeObject(this);
            File.WriteAllText(path, text);
        }
        public static Settings Load(string path = "settings.json")
        {
            Settings setting;
            if (File.Exists(path))
            {
                setting = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));
            }
            else
            {
                setting = new Settings();
            }

            return setting;
        }
    }
}
