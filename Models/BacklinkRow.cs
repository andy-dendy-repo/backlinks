using Backlinks_LE.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backlinks_LE.Models
{
    public class BacklinkRow
    {
        public DataBacklinkRow DataBacklinkRow { get; set; }
        public DataTagA DataTagA { get; set; }
        public string RespText 
        { 
            set
            {
                GetHref(value);
            }
        }

        private void GetHref(string value)
        {
            bool has = false;
            try
            {
                
                string dom = DataBacklinkRow.Domain;
                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                html.LoadHtml(value);
                var hs = html.DocumentNode.SelectNodes("//a");
                foreach (var h in hs)
                {
                        string href = h.GetAttributeValue("href", ""), rel = h.GetAttributeValue("rel", "");

                        if (href == "")
                            continue;

                        if (rel == "")
                            rel = "dofollow";

                    if (href.Contains(dom))
                    {
                        DataTagA = new DataTagA() {Anchor=h.InnerText,Href=href, Rel=rel, DataBacklinkRowID= DataBacklinkRow.ID };
                        
                        has = true;
                    }
                }
            }
            catch
            {

            }
            if(has)
                DataBacklinkRow.Status = DataBacklinkRow.Status == Status.PlainHttp ? Status.PlainHttpFound : Status.WebDriverFound;
            else
                DataBacklinkRow.Status = DataBacklinkRow.Status == Status.PlainHttp ? Status.PlainHttpNotFound : Status.WebDriverNotFound;
        }
    }
}
