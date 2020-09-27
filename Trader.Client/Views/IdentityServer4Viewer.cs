using DynamicData.Binding;
using IdentityModel.Client;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using Trader.Client.Infrastucture;
using Trader.Domain.Services;
using Volo.Abp.Identity;

namespace Trader.Client.Views
{
    public class IdentityServer4Viewer : AbstractNotifyPropertyChanged
    {
        private IIdentityService IdentityService { get; }
        private ILoginService LoginService { get; }

        public IdentityServer4Viewer(IIdentityService identityService, ILoginService loginService)
        {
            IdentityService = identityService;
            LoginService = loginService;
        }

        private TokenResponse token;
        public TokenResponse Token
        {
            get => token;
            set => SetAndRaise(ref token, value);
        }

        private UserInfoResponse userInfo;
        public UserInfoResponse UserInfo
        {
            get => userInfo;
            set => SetAndRaise(ref userInfo, value);
        }

        public Command PasswordToken => new Command(async () =>
            {
                Token = await LoginService.RequestPasswordTokenAsync();
                var token = LoginService.ShowTokens();
            });
        public Command ClientCredentialsToken => new Command(async () =>
        {
            Token = await LoginService.RequestClientCredentialsTokenAsync();
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
            string profile = await LoginService.CallServiceAsync("/api/identity/my-profile");
            IdentityUser identityUser = JsonConvert.DeserializeObject<IdentityUser>(profile);
        });
    }
}
