using DynamicData.Binding;
using System.Collections.Generic;
using System.Net.Http;
using Trader.Client.Infrastucture;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class IdentityServer4Viewer : AbstractNotifyPropertyChanged
    {
        private IIdentityService IdentityService { get; }

        public IdentityServer4Viewer(IIdentityService identityService)
        {
            IdentityService = identityService;
        }

        public Command EndSession => new Command(async () =>
          {
              HttpResponseMessage httpResponseMessage = await IdentityService.EndSessionAsync();
          });

        public Command CheckSession => new Command(async () =>
        {
            HttpResponseMessage httpResponseMessage = await IdentityService.CheckSessionAsync();
        });


        public Command Userinfo => new Command(async () =>
        {
            HttpResponseMessage httpResponseMessage = await IdentityService.GetUserinfoAsync();
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
    }
}
