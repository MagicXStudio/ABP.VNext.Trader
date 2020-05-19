using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Trader.Domain.Services
{
    public class LoginService : ILoginService, IDisposable
    {
        static string baseAddress = "http://106.13.130.51/";

        static HttpClient _tokenClient = new HttpClient();
        static DiscoveryCache _cache = new DiscoveryCache(baseAddress);

        public LoginService()
        {

        }
        public async Task<TokenResponse> RequestClientCredentialsTokenAsync()
        {
            HttpClient client = new HttpClient();
            DiscoveryDocumentRequest discoveryDoc = new DiscoveryDocumentRequest()
            {
                Address = baseAddress,
                Policy = new DiscoveryPolicy()
                {
                    RequireHttps = false,
                    ValidateIssuerName = false
                }
            };

            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(discoveryDoc);

            if (disco.IsError)
                throw new Exception(disco.Error);

            TokenResponse response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                Scope = "Magic",
                GrantType = "password",
                ClientId = "Magic_Web",
                ClientSecret = "1q2w3e*"
            });

            if (response.IsError) throw new Exception(response.Error);
            return response;
        }

        public async Task CallServiceAsync(string token)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            client.SetBearerToken(token);
            string response = await client.GetStringAsync("identity");

            Console.WriteLine(JArray.Parse(response));
        }
        public async Task<TokenResponse> RequestPasswordTokenAsync()
        {
            DiscoveryDocumentRequest discoveryDoc = new DiscoveryDocumentRequest()
            {
                Address = baseAddress,
                Policy = new DiscoveryPolicy()
                {
                    RequireHttps = false,
                }
            };
            DiscoveryDocumentResponse disco = await _tokenClient.GetDiscoveryDocumentAsync(discoveryDoc);

            if (disco.IsError)
                throw new Exception(disco.Error);

            TokenResponse response = await _tokenClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                Scope = "Magic",
                GrantType = "password",
                ClientId = "Magic_App",
                ClientSecret = "1q2w3e*",

                UserName = "Admin@10000.com",
                Password = "Admin@10000.com",

            });

            if (response.IsError)
                throw new Exception(response.Error);
            return response;
        }

        public async Task GetClaimsAsync(string token)
        {
            DiscoveryDocumentResponse disco = await _cache.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);

            UserInfoResponse response = await _tokenClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = token
            });

            if (response.IsError) throw new Exception(response.Error);


            foreach (var claim in response.Claims)
            {
                Console.WriteLine("{0}\n {1}", claim.Type, claim.Value);
            }
        }


        public void Dispose()
        {

        }
    }
}
