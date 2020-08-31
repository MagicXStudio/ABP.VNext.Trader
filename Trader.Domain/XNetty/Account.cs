using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trader.Domain.Model;
using Trader.Domain.XNetty;

namespace Abp.VNext.Hello.XNetty
{
    public class Account
    {
        /// <summary>
        /// 延迟初始化
        /// </summary>
        static Lazy<Account> lazy = new Lazy<Account>(() => new Account(), true);
        public string SessionToken => "";
        public static Account Instance => lazy.Value;

        public ChatApi ChatApi { get => new ChatApi(this); }

        public Dictionary<Tuple<int, int>, Action<string>> Actions { get; } = new Dictionary<Tuple<int, int>, Action<string>>();

        public delegate void MessageHandler(string json, Tuple<int, int> command);
        public event MessageHandler MessageEventHandler;
        private void OnCallBackEventHandler(string json, Tuple<int, int> command)
        {
            MessageEventHandler?.Invoke(json, command);
        }

        public string HeartBeatNow { get; private set; } = string.Empty;
        /// <summary>
        /// 图形验证码
        /// </summary>

        private Account()
        {
            Register();
            Actions.Add(Commands.Login, OnLogin);
        }
        /// <summary>
        /// 注册命令类型跟回调
        /// </summary>
        private void Register()
        {
            Tuple<int, int> Login = Tuple.Create(0, 1);
            Actions.Add(Login, OnLogin);
        }


        public Task LoginByTokenAsync()
        {
            return ChatApi.LoginAsync("","");

        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task LoginAsync(string account, string password)
        {
            return ChatApi.LoginAsync(password, account);

        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="password">密码</param>
        /// <param name="verificationCode">验证码</param>
        /// <param name="orgCode">组织代码</param>
        /// <returns></returns>
        public Task RegisterAsync(string phone, string password, string verificationCode, string orgCode)
        {
            return Task.Run(() => { });
        }


        /// <summary>
        /// 心跳
        /// </summary>
        /// <returns></returns>
        public Task HeartBeatAsync()
        {
            return Task.Run(() => { });
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task LogOutAsync()
        {
            return Task.Run(() => { });
        }


        /// <summary>
        /// 登录结果
        /// </summary>
        /// <param name="json"></param>
        private void OnLogin(string json)
        {
            OnCallBackEventHandler(json, Commands.Login);
        }

        /// <summary>
        /// 心跳应答
        /// </summary>
        /// <param name="json"></param>
        private void OnHeartBeat(string json)
        {

        }
        /// <summary>
        /// 与服务器时间差
        /// </summary>
        /// <param name="json"></param>
        private void OnServerTime(string json)
        {
            ReplyContent<long> replyObject = ReplyContent<long>.GetModuleInfo(json);
            long clientTime = replyObject.ClientTime;
            CoreDispatcher.Dispatcher.TimeDiff = TimeSpan.FromMilliseconds(clientTime - replyObject.Result);
        }

        public void SaveAuthToken(string authToken)
        {
            if (authToken is null)
            {
                throw new ArgumentNullException(nameof(authToken));
            }
            //  SqLiteUtils.AddSetting("Phone", authToken.Phone, "登录手机号码");

        }
    }

}
