using System;
using System.Collections.Generic;
using System.Text;

namespace PuppeteerExtraSharp.Plugins.Recaptcha
{
    public class RecaptchaResult
    {
        public bool IsSuccess { get; set; } = true;
        public CaptchaException Exception { get; set; }
    }
}
