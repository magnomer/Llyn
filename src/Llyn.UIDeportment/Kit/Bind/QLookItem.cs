using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Llyn.UIDeportment;

internal static class QLookItem
{
    private static readonly ConditionalWeakTable<
        ItemsControl, Action<FrameworkElement, object, string?>> QLookList = [];

    private static readonly ConditionalWeakTable<
        FrameworkElement, Tuple<object, PropertyChangedEventHandler>> QLookWatch = [];

    private static readonly ConditionalWeakTable<ItemsControl, HashSet<FrameworkElement>> QLookRoster = [];

    static QLookItem()
    {
        QLocalizationCatalog.QLocalizationCatalogCurrent.PropertyChanged += (_, _) =>
        {
            foreach (KeyValuePair<ItemsControl, Action<FrameworkElement, object, string?>> pair in QLookList.ToList())
            {
                QLookItemScan(pair.Key, pair.Value, true);
            }
        };
    }

    internal static void QLookItemAttach(ItemsControl list, Action<FrameworkElement, object, string?> fill)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(fill);

        if (QLookList.TryGetValue(list, out _))
        {
            return;
        }

        QLookList.Add(list, fill);
        list.ItemContainerGenerator.StatusChanged += (_, _) => QLookItemScan(list, fill, false);
        list.Loaded += (_, _) => QLookItemScan(list, fill, false);
        list.Unloaded += (_, _) => QLookItemDetach(list, []);
        QLookItemScan(list, fill, false);
    }

    internal static void QLookItemApply(ItemsControl list)
    {
        ArgumentNullException.ThrowIfNull(list);

        if (QLookList.TryGetValue(list, out Action<FrameworkElement, object, string?>? fill))
        {
            QLookItemScan(list, fill, true);
        }
    }

    private static void QLookItemScan(ItemsControl list, Action<FrameworkElement, object, string?> fill, bool full)
    {
        if (list.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
        {
            return;
        }

        HashSet<FrameworkElement> live = [];
        for (int index = 0; index < list.Items.Count; index++)
        {
            if (list.ItemContainerGenerator.ContainerFromIndex(index) is not FrameworkElement container)
            {
                continue;
            }

            object item = list.Items[index];
            live.Add(container);
            if (QLookWatch.TryGetValue(container, out Tuple<object, PropertyChangedEventHandler>? prior))
            {
                if (ReferenceEquals(prior.Item1, item))
                {
                    if (full)
                    {
                        fill(container, item, null);
                    }

                    continue;
                }

                if (prior.Item1 is INotifyPropertyChanged old)
                {
                    old.PropertyChanged -= prior.Item2;
                }

                QLookWatch.Remove(container);
            }

            fill(container, item, null);
            PropertyChangedEventHandler handler = (_, e) => fill(container, item, e.PropertyName);
            if (item is INotifyPropertyChanged watched)
            {
                watched.PropertyChanged += handler;
            }

            QLookWatch.Add(container, Tuple.Create(item, handler));
        }

        QLookItemDetach(list, live);
    }

    private static void QLookItemDetach(ItemsControl list, HashSet<FrameworkElement> live)
    {
        HashSet<FrameworkElement> roster = QLookRoster.GetValue(list, _ => []);
        foreach (FrameworkElement container in roster.Where(container => !live.Contains(container)).ToList())
        {
            if (QLookWatch.TryGetValue(container, out Tuple<object, PropertyChangedEventHandler>? prior))
            {
                if (prior.Item1 is INotifyPropertyChanged old)
                {
                    old.PropertyChanged -= prior.Item2;
                }

                QLookWatch.Remove(container);
            }

            roster.Remove(container);
        }

        roster.UnionWith(live);
    }
}
