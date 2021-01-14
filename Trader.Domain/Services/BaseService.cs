using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Trader.Domain.Services
{
    public abstract class BaseService : IDisposable
    {
        public string AuthServer { get; private set; } = "http://106.13.130.51:6060";

        public string BaseAddress { get; private set; } = "http://106.13.130.51:7070";

        public DiscoveryCache DiscoveryCache { get; set; }

        public HttpClient AuthHttpClient { get; private set; }
        public HttpClient HttpClient { get; private set; }

        TokenResponse httpToken;
        public TokenResponse HttpToken
        {
            get => httpToken;
            set
            {
                httpToken = value;
                Write(httpToken.AccessToken);
            }
        }

        public BaseService()
        {
            AuthHttpClient = new HttpClient() { BaseAddress = new Uri(AuthServer) };
            HttpClient = new HttpClient() { BaseAddress = new Uri(BaseAddress) };
            // HttpClient.DefaultRequestHeaders.Add("xtenant", "mafeiyang");
            DiscoveryCache = new DiscoveryCache(AuthServer, new DiscoveryPolicy()
            {
                RequireHttps = false,
                ValidateIssuerName = false,
                ValidateEndpoints = false
            })
            { CacheDuration = TimeSpan.FromDays(1) };
        }

        public async Task<string> GetAsync(string path)
        {
            string token = await Read();
            HttpClient.SetBearerToken(token);
            return await HttpClient.GetStringAsync(path);
        }
        public async Task<HttpResponseMessage> PostAsync(string path, HttpMethod method, StringContent content)
        {
            string token = await Read();
            AuthHttpClient.SetBearerToken(token);
            return await HttpClient.PostAsync(path, content);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public DiscoveryDocumentRequest DiscoveryDocument => new DiscoveryDocumentRequest()
        {
            Address = AuthServer,
            Policy = new DiscoveryPolicy()
            {
                RequireHttps = false,
                ValidateIssuerName = false,
                ValidateEndpoints = false
            }
        };

        public Task Write(string token) => File.WriteAllTextAsync("./AccessToken.txt", token);
        public Task<string> Read() => File.ReadAllTextAsync("./AccessToken.txt");

        public (JObject header, JObject claim) DecodeToken(TokenResponse token)
        {
            if (!token.AccessToken.Contains("."))
            {
            }
            string[] parts = token.AccessToken.Split('.');
            string header = parts[0];
            string claims = parts[1];
            byte[] h = Base64Url.Decode(header);
            byte[] c = Base64Url.Decode(claims);
            JObject s = JObject.Parse(Encoding.UTF8.GetString(h));
            JObject b = JObject.Parse(Encoding.UTF8.GetString(c));
            return (s, b);
        }
    }
}
