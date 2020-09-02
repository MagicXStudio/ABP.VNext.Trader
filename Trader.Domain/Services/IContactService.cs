using System.Collections.Generic;
using System.Threading.Tasks;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public  interface IContactService
    {
        Task<List<ContactItem>> GetItemsAsync(string path);

    }
}
