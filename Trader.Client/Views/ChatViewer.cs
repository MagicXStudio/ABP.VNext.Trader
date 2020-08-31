using Abp.VNext.Hello.XNetty;
using ReactiveUI;
using System;
using System.Windows.Input;
using Trader.Client.Infrastucture;

namespace Trader.Client.Views
{
    public class ChatViewer : ReactiveObject
    {
        public Action<string> EnterPressEvent = async (text) =>
        {
            await CoreDispatcher.Dispatcher.Account.LoginAsync("abc", "123");
        };

        private string input;
        public string Input
        {
            get => input;
            set => this.RaiseAndSetIfChanged(ref input, value);
        }
        public string Output
        {
            get => output;
            set => this.RaiseAndSetIfChanged(ref output, value);
        }

        private string _searchText;
        private string output;

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public ICommand EnterKeyCommand { get; set; }
        public ChatViewer()
        {
            CoreDispatcher.Dispatcher.Account.MessageEventHandler += Account_MessageEventHandler;
            EnterKeyCommand = new Command(EnterKeyAction);
            HelloCommand = new Command(() =>
            {
                SearchText = DateTime.Now.ToString();
            });
        }

        private void Account_MessageEventHandler(string json, Tuple<int, int> type)
        {
            Output = Output + "\n" + json;
        }

        public Command HelloCommand { get; set; }

        public void EnterKeyAction()
        {
            string input = Input ?? "";
            Output = Output + "\n" + input;
            EnterPressEvent(input);
            Input = "";
        }
    }
}
