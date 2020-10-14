using System;
using System.Collections.Generic;
using System.Text;

namespace PuppeteerExtraSharp.Plugins.Recaptcha
{
    public class CaptchaOptions
    {
        public bool VisualFeedBack { get; set; } = false;
        public bool IsThrowException { get; set; } = false;
    }
}
