using System.Threading.Tasks;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public interface ITransactionService
    {
        Task<int> Transfer(TransactionItem from, TransactionItem to, TransactionOptions options);

        Task<int> Withdraw(string userId, decimal amount);
    }
}
