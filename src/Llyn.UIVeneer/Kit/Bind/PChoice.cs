using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

internal static class PChoice
{
    private static readonly LReferenceKind[] PChoiceMenuOrder =
    [
        LReferenceKind.LReferenceKindUnspecified,
        LReferenceKind.LReferenceKindBook,
        LReferenceKind.LReferenceKindJournal,
        LReferenceKind.LReferenceKindArticle,
        LReferenceKind.LReferenceKindWeb,
        LReferenceKind.LReferenceKindVideo,
        LReferenceKind.LReferenceKindAudio,
        LReferenceKind.LReferenceKindPicture,
        LReferenceKind.LReferenceKindOther,
        LReferenceKind.LReferenceKindUnknown,
    ];

    private static readonly IReadOnlyDictionary<LCatalogOrder, string> PChoiceOrderName =
        new Dictionary<LCatalogOrder, string>
    {
        [LCatalogOrder.LCatalogOrderName] = "Name",
        [LCatalogOrder.LCatalogOrderHeadword] = "Headword",
        [LCatalogOrder.LCatalogOrderReverse] = "Reverse",
        [LCatalogOrder.LCatalogOrderRecent] = "Recent",
        [LCatalogOrder.LCatalogOrderEarliest] = "Earliest",
        [LCatalogOrder.LCatalogOrderYear] = "Year",
        [LCatalogOrder.LCatalogOrderAuthor] = "Author",
        [LCatalogOrder.LCatalogOrderUsage] = "Usage",
        [LCatalogOrder.LCatalogOrderLanguage] = "Language",
        [LCatalogOrder.LCatalogOrderMarked] = "Marked",
        [LCatalogOrder.LCatalogOrderText] = "Text",
        [LCatalogOrder.LCatalogOrderSource] = "Source",
        [LCatalogOrder.LCatalogOrderKind] = "Kind",
        [LCatalogOrder.LCatalogOrderSound] = "Sound",
        [LCatalogOrder.LCatalogOrderPending] = "Pending",
        [LCatalogOrder.LCatalogOrderWork] = "Work",
        [LCatalogOrder.LCatalogOrderGrasp] = "Grasp",
    };

    internal static void PChoiceOrderBuild(
        Panel list, string prefix, RoutedEventHandler handler, IReadOnlyList<LCatalogOrder> orders)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(prefix);
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(orders);

        list.Children.Clear();
        foreach (LCatalogOrder order in orders)
        {
            RadioButton choice = new()
            {
                GroupName = list.Name,
                Tag = order,
            };
            choice.SetResourceReference(FrameworkElement.StyleProperty, "Theme.Choice.Order");
            string name = PChoiceOrderName[order];
            choice.SetResourceReference(ContentControl.ContentProperty, prefix + "." + name);
            choice.Click += handler;
            list.Children.Add(choice);
        }
    }

    internal static void PChoiceOrderApply(Popup dropdown, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(dropdown);

        foreach (RadioButton button in PChoiceButtonScan(dropdown.Child))
        {
            button.IsChecked = order.Equals(button.Tag);
        }
    }

    internal static void PChoiceFilterBuild(
        Panel list,
        IReadOnlyList<string> languages,
        LCatalogFilter filter,
        RoutedEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentNullException.ThrowIfNull(handler);

        list.Children.Clear();
        foreach (string language in languages)
        {
            CheckBox box = new()
            {
                Style = (Style)list.FindResource("Theme.Choice.Filter"),
                Tag = language,
                IsChecked = filter.LCatalogFilterMatch(language),
                Content = PChoiceRowBuild(language),
            };
            box.Click += handler;
            list.Children.Add(box);
        }
    }

    internal static void PChoiceKindBuild(Panel list, LCatalogFilter filter, RoutedEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentNullException.ThrowIfNull(handler);

        list.Children.Clear();
        foreach (LReferenceKind kind in Enum.GetValues<LReferenceKind>())
        {
            CheckBox box = new()
            {
                Style = (Style)list.FindResource("Theme.Choice.Filter"),
                Tag = LReference.LReferenceKindFormat(kind),
                IsChecked = filter.LCatalogFilterMatch(LReference.LReferenceKindFormat(kind)),
            };
            box.SetResourceReference(ContentControl.ContentProperty, LReference.LReferenceKindResolve(kind));
            box.Click += handler;
            list.Children.Add(box);
        }
    }

    internal static void PChoiceMenuBuild(Panel list, RoutedEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(handler);

        list.Children.Clear();
        foreach (LReferenceKind kind in PChoiceMenuOrder)
        {
            RadioButton choice = new()
            {
                GroupName = nameof(PChoiceMenuBuild),
                Tag = LReference.LReferenceKindFormat(kind),
            };
            choice.SetResourceReference(FrameworkElement.StyleProperty, "Theme.Choice.Order");
            choice.SetResourceReference(ContentControl.ContentProperty, LReference.LReferenceKindResolve(kind));
            choice.Click += handler;
            list.Children.Add(choice);
        }
    }

    internal static void PChoiceMenuApply(Panel list, string tag)
    {
        ArgumentNullException.ThrowIfNull(list);

        foreach (object child in list.Children)
        {
            if (child is RadioButton choice)
            {
                choice.IsChecked = string.Equals(choice.Tag as string, tag, StringComparison.Ordinal);
            }
        }
    }

    private static Grid PChoiceRowBuild(string language)
    {
        Grid row = new();
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        Grid badge = new() { Width = 19, Height = 14, VerticalAlignment = VerticalAlignment.Center };
        ImageSource? flag = LEnsignImage.LEnsignFind(language);
        if (flag is not null)
        {
            badge.Children.Add(new Image { Stretch = Stretch.Uniform, Source = flag });
        }
        else
        {
            badge.Children.Add(new Ellipse
            {
                Width = 12,
                Height = 12,
                Stroke = (Brush)row.FindResource("Theme.Muted"),
                StrokeThickness = 1.2,
            });
        }

        row.Children.Add(badge);

        TextBlock name = new()
        {
            Text = language,
            Margin = new Thickness(8, 0, 8, 0),
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetColumn(name, 1);
        row.Children.Add(name);

        return row;
    }

    private static IEnumerable<RadioButton> PChoiceButtonScan(DependencyObject? root)
    {
        if (root is null)
        {
            yield break;
        }

        if (root is RadioButton button)
        {
            yield return button;
            yield break;
        }

        foreach (object child in LogicalTreeHelper.GetChildren(root))
        {
            if (child is not DependencyObject node)
            {
                continue;
            }

            foreach (RadioButton found in PChoiceButtonScan(node))
            {
                yield return found;
            }
        }
    }
}
