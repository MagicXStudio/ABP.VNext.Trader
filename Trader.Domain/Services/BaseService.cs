using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Trader.Domain.Services
{
    public abstract class BaseService : IDisposable
    {
        public string BaseAddress { get; private set; } = "http://106.75.247.121";

        public DiscoveryCache DiscoveryCache { get; set; }

        public HttpClient HttpClient { get; private set; }

        public TokenResponse HttpToken { get; set; }

        public BaseService()
        {
            HttpClient = new HttpClient() { BaseAddress = new Uri(BaseAddress) };
            HttpClient.DefaultRequestHeaders.Add("xtenant", "mafeiyang");
            DiscoveryCache = new DiscoveryCache(BaseAddress);
            Task.Run(() =>
            {
                HttpClient.GetAsync($"/index?t={Environment.TickCount64}");
            });
        }

        public Task<string> CallServiceAsync(string path)
        {
            HttpClient.SetBearerToken(HttpToken.AccessToken);
            return HttpClient.GetStringAsync(path);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
