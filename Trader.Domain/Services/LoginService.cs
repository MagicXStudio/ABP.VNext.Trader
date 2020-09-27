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
            DiscoveryDocumentResponse disco = await HttpClient.GetDiscoveryDocumentAsync(DiscoveryDocument);

            if (disco.IsError)
                throw new Exception(disco.Error);

            TokenResponse response = await HttpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                Scope = "Dashboard profile openid",
                GrantType = "password",
                ClientId = "Dashboard_App",
                ClientSecret = "1q2w3e*"
            });

            if (response.IsError) throw new Exception(response.Error);
            return HttpToken = response;
        }


        public async Task<TokenResponse> RequestPasswordTokenAsync()
        {
            DiscoveryDocumentResponse disco = await HttpClient.GetDiscoveryDocumentAsync(DiscoveryDocument);

            if (disco.IsError)
                throw new Exception(disco.Error);
            TokenResponse response = await HttpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                Scope = "Dashboard profile openid",
                GrantType = "password",
                ClientId = "Dashboard_App",
                ClientSecret = "1q2w3e*",

                UserName = "Lucy@163.com",
                Password = "Lucy@163.com",
            });

            if (response.IsError)
                throw new Exception(response.Error);
            return HttpToken = response;
        }

     

        public string ShowTokens()
        {
            (JObject header, JObject claim) = DecodeToken(HttpToken);
            return $"{{header:{header},claim: {claim}}}";
        }
    }
}
