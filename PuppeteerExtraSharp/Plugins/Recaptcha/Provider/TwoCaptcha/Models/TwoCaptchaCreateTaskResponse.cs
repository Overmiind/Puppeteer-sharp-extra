﻿namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.TwoCaptcha.Models;

internal class TwoCaptchaCreateTaskResponse
{
    public int ErrorId { get; set; }
    public ulong TaskId { get; set; }
}

internal class TwoCaptchaGetTaskResult
{
    public int ErrorId { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorDescription { get; set; }
    public string Status { get; set; }
    public TwoCaptchaSolution Solution { get; set; }
    public string Cost { get; set; }
    public string Ip { get; set; }
    public long CreateTime { get; set; }
    public long EndTime { get; set; }
    public int SolveCount { get; set; }
}

internal class TwoCaptchaSolution
{
    public string GRecaptchaResponse { get; set; }
    public string Token { get; set; }
}