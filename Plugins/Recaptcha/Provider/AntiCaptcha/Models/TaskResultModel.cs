using System;
using System.Collections.Generic;
using System.Text;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha
{
    public class Solution    {
        public string gRecaptchaResponse { get; set; }
    }

    public class TaskResultModel    {
        public int errorId { get; set; } 
        public string status { get; set; } 
        public Solution solution { get; set; } 
        public string cost { get; set; } 
        public string ip { get; set; } 
        public int createTime { get; set; } 
        public int endTime { get; set; } 
        public string solveCount { get; set; } 
    }

}
