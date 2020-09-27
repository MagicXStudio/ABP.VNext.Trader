using IdentityModel.Client;
using System.Threading.Tasks;

namespace Trader.Domain.Services
{
    public interface ILoginService
    {
        Task<TokenResponse> RequestClientCredentialsTokenAsync();

        Task<TokenResponse> RequestPasswordTokenAsync();

        Task<string> CallServiceAsync(string path);

        string ShowTokens();
    }
}
