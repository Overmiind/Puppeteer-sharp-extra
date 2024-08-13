namespace PuppeteerExtraSharp.Plugins.Recaptcha
{
    public class RecaptchaResult
    {
        public string result { get; set; } = "";
        public bool IsSuccess { get; set; } = true;
        public CaptchaException Exception { get; set; }
    }
}
