using DynamicData.Binding;
using System;
using System.Windows.Input;
using Trader.Client.Infrastucture;

namespace Trader.Client.Views
{
    public class ChatViewer : AbstractNotifyPropertyChanged
    {
        public Action<string> EnterPressEvent = (a) =>
        {

        };
        public string Input { get; set; }
        public string Output { get; set; }
        public ICommand EnterKeyCommand { get; set; }
        public ChatViewer()
        {
            EnterKeyCommand = new Command(EnterKeyAction);
        }

        public Command RelayCommand { get; set; }

        public void EnterKeyAction()
        {
            string input = Input ?? "";
            Output = Output + "\n" + input;
            EnterPressEvent(input);
            Input = "";
        }

        public void writeOutput(string msg)
        {
            string input = msg ?? "";
            Output = Output + "\n" + input;
        }
    }
}
