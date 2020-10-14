using System;
using System.Collections.Generic;
using System.Text;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha
{
    public class TaskModel
    {
        public string clientKey { get; set; }
        public Task task { get; set; }
    }

    public class RequestForResultTask
    {
        public string clientKey { get; set; }
        public int taskId { get; set; }
    }

    public class TaskResult
    {
        public int errorId { get; set; }
        public int taskId { get; set; }
    }


    public class Task
    {
        public string type { get; set; }
        public string websiteURL { get; set; }
        public string websiteKey { get; set; }
    }
}
