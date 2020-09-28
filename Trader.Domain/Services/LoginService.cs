using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Trader.Domain.Services
{
    public class LoginService : BaseService, ILoginService
    {
        private IConfigurationRoot Configuration { get; }
        public LoginService(IConfigurationRoot configurationRoot)
        {
            Configuration = configurationRoot;
        }

        public Task<TokenResponse> RequestAuthorizationCodeTokenAsync(string code, string redirectUri, string codeVerifier)
        {
            return HttpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest()
            {
                Code = code,
                RedirectUri = redirectUri,
                CodeVerifier = codeVerifier,
            });
        }

        public async Task<TokenResponse> RequestClientCredentialsTokenAsync()
        {
            DiscoveryDocumentResponse disco = await HttpClient.GetDiscoveryDocumentAsync(DiscoveryDocument);

            if (disco.IsError)
                throw new Exception(disco.Error);

            TokenResponse response = await HttpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                Scope = Configuration["Scope"],
                GrantType = Configuration["GrantType"],
                ClientId = Configuration["ClientId"],
                ClientSecret = Configuration["ClientSecret"],
            });

            if (response.IsError) throw new Exception(response.Error);
            return HttpToken = response;
        }

        public Task<TokenResponse> RequestDeviceTokenAsync(string deviceCode)
        {
            return HttpClient.RequestDeviceTokenAsync(new DeviceTokenRequest()
            {
                DeviceCode = deviceCode
            });
        }

        public async Task<TokenResponse> RequestPasswordTokenAsync(string userName, string password)
        {
            DiscoveryDocumentResponse disco = await HttpClient.GetDiscoveryDocumentAsync(DiscoveryDocument);



            if (disco.IsError)
                throw new Exception(disco.Error);
            TokenResponse response = await HttpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                Scope = Configuration["Scope"],
                GrantType = Configuration["GrantType"],
                ClientId = Configuration["ClientId"],
                ClientSecret = Configuration["ClientSecret"],

                UserName =userName,
                Password =password,
            });

            if (response.IsError)
                throw new Exception(response.Error);
            return HttpToken = response;
        }

        public Task<TokenResponse> RequestRefreshTokenAsync(string refreshToken, string scope)
        {
            return HttpClient.RequestRefreshTokenAsync(new RefreshTokenRequest()
            {
                RefreshToken = refreshToken,
                Scope = scope
            });
        }
        public async Task<TokenResponse> RequestTokenAsync()
        {
            DiscoveryDocumentResponse disco = await DiscoveryCache.GetAsync();
            return await HttpClient.RequestTokenAsync(new TokenRequest()
            {
                Address = disco.TokenEndpoint,
                GrantType = Configuration["GrantType"],
                ClientId = Configuration["ClientId"],
                ClientSecret = Configuration["ClientSecret"],
            });
        }

        public Task<TokenResponse> RequestTokenExchangeTokenAsync()
        {
            return HttpClient.RequestTokenExchangeTokenAsync(new TokenExchangeTokenRequest()
            {

            });

        }

        public Task<TokenResponse> RequestTokenRawAsync()
        {
            return HttpClient.RequestTokenRawAsync("", new Dictionary<string, string>()
            {


            });
        }


        public async Task<DeviceAuthorizationResponse> RequestDeviceAuthorizationAsync()
        {
            DiscoveryDocumentResponse disco = await DiscoveryCache.GetAsync();
            if (disco.IsError)
                throw new Exception(disco.Error);

            DeviceAuthorizationResponse response = await HttpClient.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = disco.DeviceAuthorizationEndpoint,
                Scope = Configuration["Scope"],
                ClientId = Configuration["ClientId"],
                ClientSecret = Configuration["ClientSecret"],
            });
            Process.Start(new ProcessStartInfo(response.VerificationUri) { UseShellExecute = true });
            return response;
        }

        public async Task<TokenIntrospectionResponse> IntrospectTokenAsync(string accessToken)
        {
            DiscoveryDocumentResponse disco = await DiscoveryCache.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);
            TokenIntrospectionResponse result = await HttpClient.IntrospectTokenAsync(
                new TokenIntrospectionRequest
                {
                    Address = disco.IntrospectionEndpoint,
                    ClientId = Configuration["ClientId"],
                    ClientSecret = Configuration["ClientSecret"],
                    Token = accessToken
                });
            return result;
        }


        public string ShowTokens()
        {
            (JObject header, JObject claim) = DecodeToken(HttpToken);
            return $"{{header:{header},claim: {claim}}}";
        }
    }
}
