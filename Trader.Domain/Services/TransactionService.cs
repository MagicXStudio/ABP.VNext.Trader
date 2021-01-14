using System.Threading.Tasks;
using Trader.Domain.Model;
using Dapper;
using MySql.Data.MySqlClient;
using System;

namespace Trader.Domain.Services
{

    /// <summary>
    /// 原子性（A），一致性（C），隔离性（I）和持久性（D）
    /// </summary>
    public class TransactionService : ITransactionService
    {
        const string connStr = "Database=ABP;Data Source=47.98.226.195; User Id=root; Password=zxcvbnm;charset=gb2312;pooling=true;";

        public Task<int> Transfer(TransactionItem from, TransactionItem to, TransactionOptions options)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                string sql = @"
                                INSERT INTO TransactionItem
                                (`Id`,
                                `UserId`,
                                `AccountNo`,
                                `BankNo`,
                                `Amount`,
                                `Status`,
                                `LastUpdateTime`,
                                `Comment`)
                                VALUES
                                (@Id,
                                @UserId,
                                @AccountNo,
                                @BankNo,
                                @Amount,
                                @Status,
                                @LastUpdateTime,
                                @Comment);
                                ";
                return conn.ExecuteAsync(sql, new
                {
                    from.Id,
                    from.UserId,
                    from.AccountNo,
                    from.BankNo,
                    from.Amount,
                    from.Status,
                    from.LastUpdateTime,
                    from.Comment
                });
            }
        }

        public Task<int> Withdraw(string userId, decimal amount)
        {
            string sql = $@"
                         UPDATE UserAccount
                         SET
                         Balance =Balance-{amount},
                         LastUpdateTime= NOW()
                         WHERE UserId=@UserId and Balance-{amount}>0
                         limit 1;
                          ";
            Task<int> r = Task.FromResult(0);
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlTransaction tran = conn.BeginTransaction();
                try
                {
                    r = conn.ExecuteAsync(sql, new { UserId = userId }, tran);
                    tran.Commit();
                    return r;
                }
                catch
                {
                    tran.Rollback();
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return r;
        }
    }
}
