using PuppeteerSharp;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerExtraSharp.Plugins.Proxies
{
    public class ProxiesPlugin(ProxyInfo proxy) : PuppeteerExtraPlugin("proxies")
    {
        private ProxyInfo Proxy { get; } = proxy;

        public override void BeforeLaunch(LaunchOptions options)
        {
            var args = options.Args.Where(a => !a.StartsWith("--proxy-server=")).ToList();
            args.Add("--proxy-server=" + Proxy.Ip + ":" + Proxy.Port);

            options.Args = [..args];
        }

        public override async Task OnPageCreated(IPage page)
        {
            await page.AuthenticateAsync(new()
            {
                Username = Proxy.Login,
                Password = Proxy.Password,
            });
        }
    }
}
