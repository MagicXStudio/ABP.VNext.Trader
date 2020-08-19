using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using System;

namespace Trader.Domain.Services
{
    public class IdentityService : BaseService, IIdentityService
    {
        public IdentityService()
        {
            UriEndPoint endPoint = new UriEndPoint(new Uri("http://localhost:8888"));
        }

        public Task<HttpResponseMessage> CheckSessionAsync()
        {
            return HttpClient.GetAsync("/connect/checksession");
        }

        public Task<HttpResponseMessage> EndSessionAsync()
        {
            return HttpClient.GetAsync("/connect/endsession");
        }

        public Task<HttpResponseMessage> GetUserinfoAsync()
        {
            return HttpClient.GetAsync("/connect/userinfo");
        }

        public Task<HttpResponseMessage> RevocationAsync(IEnumerable<KeyValuePair<string, string>> values)
        {
            string json = JsonConvert.SerializeObject(values);
            return HttpClient.PostAsync("/connect/revocation", new StringContent(json, Encoding.UTF8, "text/json"));
        }
        public Task<HttpResponseMessage> IntrospectAsync(IEnumerable<KeyValuePair<string, string>> values)
        {
            string json = JsonConvert.SerializeObject(values);
            return HttpClient.PostAsync("/connect/introspect", new StringContent(json, Encoding.UTF8, "text/json"));
        }
    }
}
