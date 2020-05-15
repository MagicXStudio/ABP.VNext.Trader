using IdentityModel.Client;
using System.Threading.Tasks;

namespace Trader.Domain.Services
{
    public  interface ILoginService
    {

        Task<TokenResponse> RequestTokenAsync();

        Task CallServiceAsync(string token);

    }
}
