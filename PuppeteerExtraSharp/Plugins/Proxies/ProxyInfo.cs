namespace PuppeteerExtraSharp.Plugins.Proxies
{
#nullable enable
    public class ProxyInfo(string ip, string port, string? login = null, string? password = null)
    {
        public string Ip { get; set; } = ip;
        public string Port { get; set; } = port;
        public string? Login { get; set; } = login;
        public string? Password { get; set; } = password;
    }
#nullable disable
}
