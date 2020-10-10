using System.Diagnostics;

namespace Trader.Client.Infrastucture
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public readonly  struct Link
    {
        public Link(string text, string url)
            : this(text, url, url)
        {
            Text = text;
            Url = url;
        }

        public Link(string text, string display, string url)
        {
            Text = text;
            Display = display;
            Url = url;
        }


        public string Text { get; }

        public string Url { get; }

        public string Display { get; }

        private string GetDebuggerDisplay() => $"Text:{Text}Url:{Url}Display:{Display}";
    }
}