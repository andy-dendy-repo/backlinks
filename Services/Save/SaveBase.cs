using Backlinks_LE.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backlinks_LE.Services.Save
{
    public abstract class SaveBase
    {
        protected DataBacklinkRowService _serv;
        protected DataListedInfoService _infoServ;
        protected DataTagAService _tagServ;
        protected Settings _settings;
        public SaveBase(DataBacklinkRowService service, DataListedInfoService infoService, DataTagAService tagServ, Settings settings)
        {
            _serv = service;
            _infoServ = infoService;
            _tagServ = tagServ;
            _settings = settings;
        }
    }
}
