using IdentityModel.Client;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Trader.Domain.Services
{
    public interface IIdentityService
    {
        Task<HttpResponseMessage> CheckSessionAsync();

        Task<string> EndSessionAsync();

        Task<UserInfoResponse> GetUserinfoAsync();

        Task<HttpResponseMessage> RevocationAsync(IEnumerable<KeyValuePair<string, string>> values);

        Task<HttpResponseMessage> IntrospectAsync(IEnumerable<KeyValuePair<string, string>> values);
    }
}
