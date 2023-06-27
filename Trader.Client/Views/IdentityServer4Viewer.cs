using DynamicData.Binding;
using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Trader.Client.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;
using Volo.Abp.Identity;

namespace Trader.Client.Views
{
    public class IdentityServer4Viewer : AbstractNotifyPropertyChanged
    {
        private IIdentityService IdentityService { get; }
        private ILoginService LoginService { get; }
        public ITransactionService TransactionService { get; }

        public IdentityServer4Viewer(IIdentityService identityService, ILoginService loginService, ITransactionService transactionService)
        {
            IdentityService = identityService;
            LoginService = loginService;
            TransactionService = transactionService;
        }

        private TokenResponse token;
        public TokenResponse Token
        {
            get => token;
            set => SetAndRaise(ref token, value);
        }

        DeviceAuthorizationResponse deviceAuthorization;
        public DeviceAuthorizationResponse DeviceAuthorization
        {
            get => deviceAuthorization;
            set => SetAndRaise(ref deviceAuthorization, value);
        }

        private UserInfoResponse userInfo;
        public UserInfoResponse UserInfo
        {
            get => userInfo;
            set => SetAndRaise(ref userInfo, value);
        }

        public Command AuthorizationCode => new Command(async () =>
        {
            Token = await LoginService.RequestAuthorizationCodeTokenAsync(DeviceAuthorization.DeviceCode, DeviceAuthorization.VerificationUri, DeviceAuthorization.UserCode);
        });

        public Command ClientCredentialsToken => new Command(async () =>
        {
            Token = await LoginService.RequestClientCredentialsTokenAsync();
        });
        public Command DeviceToken => new Command(async () =>
        {
            Token = await LoginService.RequestDeviceTokenAsync(DeviceAuthorization.DeviceCode);
        });

        public Command PasswordToken => new Command(async () =>
        {
            Token = await LoginService.RequestPasswordTokenAsync("123@ChuangYu.com", "123@ChuangYu.com");
        });

        public Command RefreshToken => new Command(async () =>
            {
                Token = await LoginService.RequestRefreshTokenAsync(Token.AccessToken, Token.Scope);
            });
        public Command RequestToken => new Command(async () =>
        {
            Token = await LoginService.RequestTokenAsync();
        });
        public Command TokenExchangeToken => new Command(async () =>
        {
            Token = await LoginService.RequestTokenExchangeTokenAsync();
        });
        public Command TokenRaw => new Command(async () =>
        {
            Token = await LoginService.RequestTokenRawAsync();
        });
        public Command DeviceAuthorizationCommand => new Command(async () =>
        {
            DeviceAuthorization = await LoginService.RequestDeviceAuthorizationAsync();
        });
        public Command IntrospectToken => new Command(async () =>
        {
            TokenIntrospectionResponse TokenIntrospection = await LoginService.IntrospectTokenAsync(Token.AccessToken);
        });

        public Command EndSession => new Command(async () =>
          {
              string httpResponseMessage = await IdentityService.EndSessionAsync();
          });

        public Command CheckSession => new Command(async () =>
        {
            HttpResponseMessage httpResponseMessage = await IdentityService.CheckSessionAsync();
        });

        public Command Userinfo => new Command(async () =>
        {
            UserInfoResponse userInfo = await IdentityService.GetUserinfoAsync();
        });

        public Command Revocation => new Command(async () =>
        {
            IEnumerable<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();

            HttpResponseMessage httpResponseMessage = await IdentityService.RevocationAsync(values);
        });

        public Command Introspect => new Command(async () =>
        {
            IEnumerable<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
            HttpResponseMessage httpResponseMessage = await IdentityService.IntrospectAsync(values);
        });

        public Command Profile => new Command(async () =>
        {
            string profile = await LoginService.GetAsync("/api/app/management/units");
            IdentityUser identityUser = JsonConvert.DeserializeObject<IdentityUser>(profile);
        });

        public Command Transfer => new Command(async () =>
        {
            await TransactionService.Transfer(new TransactionItem()
            {
                Id = Environment.TickCount.ToString(),
                UserId = $"{DateTime.Now.Second}",
                AccountNo = $"{DateTimeOffset.Now.ToUnixTimeSeconds()}",
                BankNo = "ICBC",
                Amount = DateTime.Now.Millisecond,
                Status = 1,
                LastUpdateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Comment = $"{DateTime.Now}"
            }, new TransactionItem() { }, new TransactionOptions());
            await TransactionService.Withdraw($"{Thread.CurrentThread.ManagedThreadId}", Thread.CurrentThread.ManagedThreadId);
        });

        public Command Withdraw => new Command(async () =>
        {
            await TransactionService.Withdraw($"{Thread.CurrentThread.ManagedThreadId}", Thread.CurrentThread.ManagedThreadId);
        });

        public Command ShowTokens => new Command(async () =>
       {
           string s = await LoginService.ShowTokensAsync();
           System.Diagnostics.Debug.Write(s);
       });
    }
}
