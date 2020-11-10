using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Trader.Domain.Services
{
    public class IdentityService : BaseService, IIdentityService
    {
        public IdentityService()
        {
        }

        public Task<HttpResponseMessage> CheckSessionAsync()
        {
            return HttpClient.GetAsync("/connect/checksession");
        }

        public Task<string> EndSessionAsync()
        {
            return GetAsync("/connect/endsession");
        }
       
        public async Task<UserInfoResponse> GetUserinfoAsync()
        {
            DiscoveryDocumentResponse disco = await DiscoveryCache.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);

            UserInfoResponse response = await HttpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = await Read()
            }); ;

            if (response.IsError) throw new Exception(response.Error);
            return response;
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
