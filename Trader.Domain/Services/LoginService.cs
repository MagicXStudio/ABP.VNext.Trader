using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Trader.Domain.Services
{
    public class LoginService : ILoginService, IDisposable
    {
        public LoginService()
        {

        }
        public async Task<TokenResponse> RequestTokenAsync()
        {
            HttpClient client = new HttpClient();
            DiscoveryDocumentRequest discoveryDoc = new DiscoveryDocumentRequest()
            {
                Address = "http://106.13.130.51/",
                Policy = new DiscoveryPolicy()
                {
                    RequireHttps = false,
                }
            };

            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(discoveryDoc);

            if (disco.IsError)
                throw new Exception(disco.Error);

            TokenResponse response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                Scope = "XManagement",
                GrantType = "password",
                ClientId = "XManagement_App",
                ClientSecret = "1q2w3e*"
            });

            if (response.IsError) throw new Exception(response.Error);
            return response;
        }

        public async Task CallServiceAsync(string token)
        {
            string baseAddress = "http://106.13.130.51/";

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            client.SetBearerToken(token);
            string response = await client.GetStringAsync("identity");

            Console.WriteLine(JArray.Parse(response));
        }


        public void Dispose()
        {

        }
    }
}
