using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Llyn.UIDeportment;

internal static class PLookItem
{
    private static readonly ConditionalWeakTable<
        ItemsControl, Action<FrameworkElement, object, string?>> PLookList = [];

    private static readonly ConditionalWeakTable<
        FrameworkElement, Tuple<object, PropertyChangedEventHandler>> PLookWatch = [];

    private static readonly ConditionalWeakTable<ItemsControl, HashSet<FrameworkElement>> PLookRoster = [];

    static PLookItem()
    {
        PLocalizationCatalog.PLocalizationCatalogCurrent.PropertyChanged += (_, _) =>
        {
            foreach (KeyValuePair<ItemsControl, Action<FrameworkElement, object, string?>> pair in PLookList.ToList())
            {
                PLookItemScan(pair.Key, pair.Value, true);
            }
        };
    }

    internal static void PLookItemAttach(ItemsControl list, Action<FrameworkElement, object, string?> fill)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(fill);

        if (PLookList.TryGetValue(list, out _))
        {
            return;
        }

        PLookList.Add(list, fill);
        list.ItemContainerGenerator.StatusChanged += (_, _) => PLookItemScan(list, fill, false);
        list.Loaded += (_, _) => PLookItemScan(list, fill, false);
        list.Unloaded += (_, _) => PLookItemDetach(list, []);
        PLookItemScan(list, fill, false);
    }

    internal static void PLookItemApply(ItemsControl list)
    {
        ArgumentNullException.ThrowIfNull(list);

        if (PLookList.TryGetValue(list, out Action<FrameworkElement, object, string?>? fill))
        {
            PLookItemScan(list, fill, true);
        }
    }

    private static void PLookItemScan(ItemsControl list, Action<FrameworkElement, object, string?> fill, bool full)
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
            if (PLookWatch.TryGetValue(container, out Tuple<object, PropertyChangedEventHandler>? prior))
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

                PLookWatch.Remove(container);
            }

            fill(container, item, null);
            PropertyChangedEventHandler handler = (_, e) => fill(container, item, e.PropertyName);
            if (item is INotifyPropertyChanged watched)
            {
                watched.PropertyChanged += handler;
            }

            PLookWatch.Add(container, Tuple.Create(item, handler));
        }

        PLookItemDetach(list, live);
    }

    private static void PLookItemDetach(ItemsControl list, HashSet<FrameworkElement> live)
    {
        HashSet<FrameworkElement> roster = PLookRoster.GetValue(list, _ => []);
        foreach (FrameworkElement container in roster.Where(container => !live.Contains(container)).ToList())
        {
            if (PLookWatch.TryGetValue(container, out Tuple<object, PropertyChangedEventHandler>? prior))
            {
                if (prior.Item1 is INotifyPropertyChanged old)
                {
                    old.PropertyChanged -= prior.Item2;
                }

                PLookWatch.Remove(container);
            }

            roster.Remove(container);
        }

        roster.UnionWith(live);
    }
}
