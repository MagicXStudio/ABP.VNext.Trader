using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class ContactService : BaseService, IContactService
    {
        public ContactService()
        {
        }

        public async Task<List<ContactItem>> GetItemsAsync(string path)
        {
            List<ContactItem> result = new List<ContactItem>();
            string json = await HttpClient.GetStringAsync(path);
            result = JsonConvert.DeserializeObject<List<ContactItem>>(json);
            return result;
        }
    }
}
