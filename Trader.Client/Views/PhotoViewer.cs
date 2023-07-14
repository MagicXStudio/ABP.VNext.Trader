using DynamicData.Binding;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Trader.Client.Forms;
using Trader.Client.Infrastucture;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Views
{
    public class PhotoViewer : AbstractNotifyPropertyChanged
    {
        private readonly ILogger _logger;
        private readonly IObjectProvider _objectProvider;
        private bool _showLinks = false;
        private IEnumerable<TaskItem> _items;
        public string CurrentDir => Directory.GetCurrentDirectory();

        public PhotoViewer(ILogger logger, IObjectProvider objectProvider)
        {
            string[] files = Directory.GetFiles(CurrentDir + "/Assets/fashion");
            _items = files.Select(x => new TaskItem(x,
                    x,
                    () => Open(x)));

            _logger = logger;
            _objectProvider = objectProvider;
        }

        private void Open(string file)
        {
            FrmImageView view = new FrmImageView(file);
            view.Show();
            _logger.Debug("Opening '{0}'", file);
        }

        public ObservableCollection<TaskItem> Items
        {
            get => new ObservableCollection<TaskItem>(_items);
            set => SetAndRaise(ref _items, value);
        }

        public bool ShowLinks
        {
            get => _showLinks;
            set => SetAndRaise(ref _showLinks, value);
        }

        public void Dispose()
        {
        }
    }
}
