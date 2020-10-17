using System;
using System.Collections.Generic;
using System.Text;

namespace Backlinks_LE.Models
{
    public enum Status
    {
        InAQueue,
        PlainHttp,
        PlainHttpFound,
        PlainHttpNotFound,
        WebDriver,
        WebDriverFound,
        WebDriverNotFound
    }
}
