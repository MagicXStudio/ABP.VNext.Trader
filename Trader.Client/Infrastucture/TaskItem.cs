using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Input;
using DynamicData.Binding;

namespace Trader.Client.Infrastucture
{
    public class TaskItem : AbstractNotifyPropertyChanged, IComparable<TaskItem>, ICloneable
    {
        public TaskItem(
            string title,
            string description,
            Action action,
            IEnumerable<Link> link = null,
            object content = null)
        {
            Title = title;
            Description = description;
            Content = content;
            Link = link ?? Enumerable.Empty<Link>();
            Command = new Command(action);
        }

        public string Title { get; }

        public ICommand Command { get; }

        private Action Action { get; }

        public IEnumerable<Link> Link { get; }

        public string Description { get; }

        public object Content { get; }

        public MenuCategory Category { get; }

        public object Clone()
        {
            return new TaskItem(
            Title,
            Description,
            Action,
            Link,
            Command);
        }

        public int CompareTo([AllowNull] TaskItem other) => Title.Length >= other.Title.Length ? 1 : 0;
    }
}