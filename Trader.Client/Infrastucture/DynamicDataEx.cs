using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace DynamicData
{
    public static class DynamicDataEx
    {
        public static IObservable<IChangeSet<TObject, TKey>> DelayRemove<TObject, TKey>(this IObservable<IChangeSet<TObject, TKey>> source,
            TimeSpan delayPeriod, Action<TObject> onDefer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (onDefer == null) throw new ArgumentNullException(nameof(onDefer));

            return Observable.Create<IChangeSet<TObject, TKey>>(observer =>
            {

                var locker = new object();
                IConnectableObservable<IChangeSet<TObject, TKey>> shared = source.Synchronize(locker).Publish();

                var notRemoved = shared.WhereReasonsAreNot(ChangeReason.Remove);


                var removes = shared.WhereReasonsAre(ChangeReason.Remove)
                    .Do(changes => changes.Select(change => change.Current).ForEach(onDefer))
                    .Delay(delayPeriod)
                    .Synchronize(locker);

                IDisposable subscriber = notRemoved.Merge(removes).SubscribeSafe(observer);
                return new CompositeDisposable(subscriber, shared.Connect());

            });
        }

        public static IObservable<IChangeSet<TObject>> DelayRemove<TObject>(this IObservable<IChangeSet<TObject>> source,
                TimeSpan delayPeriod, Action<TObject> onDefer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (onDefer == null) throw new ArgumentNullException(nameof(onDefer));

            return Observable.Create<IChangeSet<TObject>>(observer =>
            {
                ListChangeReason[] removed = new[] { ListChangeReason.Remove, ListChangeReason.Clear, ListChangeReason.RemoveRange };

                SourceList<TObject> localList = new SourceList<TObject>();

                object locker = new object();
                IConnectableObservable<IChangeSet<TObject>> shared = source.Synchronize(locker).Publish();

                IDisposable notRemoved = shared.WhereReasonsAreNot(removed)
                                        .Subscribe(changes =>
                                        {
                                            localList.Edit(innerList =>
                                            {
                                                changes.ForEach(change =>
                                                {
                                                    switch (change.Reason)
                                                    {
                                                        case ListChangeReason.Add:
                                                            innerList.Add(change.Item.Current);
                                                            break;
                                                        case ListChangeReason.AddRange:
                                                            change.Range.ForEach(innerList.Add);
                                                            break;
                                                        case ListChangeReason.Replace:
                                                            innerList.Replace(change.Item.Previous.Value, change.Item.Current);
                                                            break;
                                                    }
                                                });
                                            });
                                        });

                //when removed,invoke call back
                IDisposable removes = shared.WhereReasonsAre(removed)
                                .ForEachItemChange(change => onDefer(change.Current))
                                .Delay(delayPeriod)
                                .Synchronize(locker)
                                .Subscribe(changes =>
                                {
                                    //flatten removes into a single enumerable
                                    TObject[] toRemove = changes.SelectMany(change =>
                                    {
                                        return change.Type == ChangeType.Item ? new[] { change.Item.Current } : change.Range.Select(t => t);
                                    }).ToArray();
                                    //remove in one hit
                                    localList.RemoveMany(toRemove);
                                });

                IDisposable subscriber = localList.Connect().SubscribeSafe(observer);
                return new CompositeDisposable(subscriber, removes, notRemoved, shared.Connect());
            });
        }
    }
}