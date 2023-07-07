using ReactiveUI;

namespace Trader.Client.Views
{
    public class LogEntryViewer : ReactiveObject
    {
        public LogEntryViewer()
        {
            
        }

        public string SearchText { get; set; }


        public IReactiveCommand DeleteCommand { get; set; }
    }
}