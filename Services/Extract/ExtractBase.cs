using Backlinks_LE.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backlinks_LE.Services.Extract
{
    public abstract class ExtractBase
    {
        protected DataBacklinkRowService _serv;
        protected DataListedInfoService _infoServ;
        protected Settings _settings;
        
    }
}
