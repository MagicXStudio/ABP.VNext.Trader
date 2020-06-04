using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Trader.Domain.Services
{
    public class LoginService : BaseService, ILoginService
    {
        public LoginService()
        {

        }
        public async Task<TokenResponse> RequestClientCredentialsTokenAsync()
        {
            HttpClient client = new HttpClient();
            DiscoveryDocumentRequest discoveryDoc = new DiscoveryDocumentRequest()
            {
                Address = BaseAddress,
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
            HttpToken = response;
            return HttpToken;
        }

        public Task CallServiceAsync(string token)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(BaseAddress)
            };
            client.SetBearerToken(token);
            return client.GetStringAsync("identity");
        }
        public async Task<TokenResponse> RequestPasswordTokenAsync()
        {
            DiscoveryDocumentRequest discoveryDoc = new DiscoveryDocumentRequest()
            {
                Address = BaseAddress,
                Policy = new DiscoveryPolicy()
                {
                    RequireHttps = false,
                }
            };
            HttpClient.DefaultRequestHeaders.Add("__tenant", "ABC");
            DiscoveryDocumentResponse disco = await HttpClient.GetDiscoveryDocumentAsync(discoveryDoc);

            if (disco.IsError)
                throw new Exception(disco.Error);
            TokenResponse response = await HttpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                Scope = "Magic",
                GrantType = "password",
                ClientId = "Magic_Web",
                ClientSecret = "1q2w3e*",

                UserName = "Discovery@163.com",
                Password = "Discovery@163.com",

            });

            if (response.IsError)
                throw new Exception(response.Error);
            return response;
        }

        public async Task GetClaimsAsync(string token)
        {
            DiscoveryDocumentResponse disco = await DiscoveryCache.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);

            UserInfoResponse response = await HttpClient.GetUserInfoAsync(new UserInfoRequest
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

    }
}
