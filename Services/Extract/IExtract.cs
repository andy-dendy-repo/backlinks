﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backlinks_LE.Services.Extract
{
    public interface IExtract
    {
        Task<int> Extract();
    }
}
