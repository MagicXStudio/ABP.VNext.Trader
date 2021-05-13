using IdentityModel.Client;
using System.Threading.Tasks;

namespace Trader.Domain.Services
{
    public interface ILoginService
    {
        Task<TokenResponse> RequestAuthorizationCodeTokenAsync(string code, string redirectUri, string codeVerifier);
        Task<TokenResponse> RequestClientCredentialsTokenAsync();
        Task<TokenResponse> RequestDeviceTokenAsync(string deviceCode);
        Task<TokenResponse> RequestPasswordTokenAsync(string userName, string password);
        Task<TokenResponse> RequestRefreshTokenAsync(string refreshToken, string scope);
        Task<TokenResponse> RequestTokenAsync();
        Task<TokenResponse> RequestTokenExchangeTokenAsync();
        Task<TokenResponse> RequestTokenRawAsync();

        Task<DeviceAuthorizationResponse> RequestDeviceAuthorizationAsync();
        Task<TokenIntrospectionResponse> IntrospectTokenAsync(string accessToken);

        Task<string> GetAsync(string path);

        Task<string> ShowTokensAsync();
    }
}
