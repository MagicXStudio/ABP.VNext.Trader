using Abp.VNext.Hello.XNetty;
using System.Threading.Tasks;
using Trader.Domain.Model;

namespace Trader.Domain.XNetty
{
    public class ChatApi
    {
        private Account Account { get; set; }

        private static ClientBootstrap Client => ClientBootstrap.Client;

        public ChatApi(Account account)
        {
            Account = account;
        }

        public Task LoginAsync(string password, string account)
        {
            RequestCommand<string> cmd = new RequestCommand<string>()
            {
                Scope = Commands.Login.Item1,
                Cmd = Commands.Login.Item2,
                Data = password,
                Message = account
            };
            return Client.ChatChannel.WriteAndFlushAsync(cmd.ToString());
        }
    }
}
