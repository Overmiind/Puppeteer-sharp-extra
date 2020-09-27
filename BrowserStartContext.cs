using System;
using System.Collections.Generic;
using System.Text;

namespace PuppeteerExtraSharp
{
    public class BrowserStartContext
    {
        public bool IsHeadless { get; set; }
        public StartType StartType { get; set; }
    }

    public enum StartType
    {
        Connect,
        Launch
    }
}
