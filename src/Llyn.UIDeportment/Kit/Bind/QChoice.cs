using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal static class QChoice
{
    private static readonly IReadOnlyDictionary<CCatalogOrder, string> QChoiceWord =
        new Dictionary<CCatalogOrder, string>
        {
            [CCatalogOrder.CCatalogOrderName] = "Name",
            [CCatalogOrder.CCatalogOrderHeadword] = "Headword",
            [CCatalogOrder.CCatalogOrderReverse] = "Reverse",
            [CCatalogOrder.CCatalogOrderRecent] = "Recent",
            [CCatalogOrder.CCatalogOrderEarliest] = "Earliest",
            [CCatalogOrder.CCatalogOrderYear] = "Year",
            [CCatalogOrder.CCatalogOrderAuthor] = "Author",
            [CCatalogOrder.CCatalogOrderUsage] = "Usage",
            [CCatalogOrder.CCatalogOrderLanguage] = "Language",
            [CCatalogOrder.CCatalogOrderMarked] = "Marked",
            [CCatalogOrder.CCatalogOrderText] = "Text",
            [CCatalogOrder.CCatalogOrderSource] = "Source",
            [CCatalogOrder.CCatalogOrderKind] = "Kind",
            [CCatalogOrder.CCatalogOrderSound] = "Sound",
            [CCatalogOrder.CCatalogOrderPending] = "Pending",
            [CCatalogOrder.CCatalogOrderWork] = "Work",
            [CCatalogOrder.CCatalogOrderGrasp] = "Grasp",
        };

    internal static void QChoiceOrderBuild(
        Panel list, string prefix, RoutedEventHandler handler, IReadOnlyList<CCatalogOrder> orders)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(prefix);
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(orders);

        list.Children.Clear();
        foreach (CCatalogOrder order in orders)
        {
            RadioButton choice = new()
            {
                GroupName = list.Name,
                Tag = QChoiceWord[order],
            };
            choice.SetResourceReference(FrameworkElement.StyleProperty, "Theme.Choice.Order");
            choice.SetResourceReference(ContentControl.ContentProperty, QChoiceKeyRead(prefix, QChoiceWord[order]));
            choice.Click += handler;
            list.Children.Add(choice);
        }
    }

    internal static void QChoiceDropperAttach(ToggleButton dropper, Popup dropdown, UIElement anchor)
    {
        ArgumentNullException.ThrowIfNull(dropper);
        ArgumentNullException.ThrowIfNull(dropdown);

        dropdown.PlacementTarget = anchor;
        dropper.Checked += (_, _) => dropdown.IsOpen = true;
        dropper.Unchecked += (_, _) => dropdown.IsOpen = false;
        dropdown.Closed += (_, _) => dropper.IsChecked = false;
    }

    internal static void QChoiceOrderApply(Popup dropdown, CCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(dropdown);

        QChoiceWordApply(dropdown, QChoiceWord[order]);
    }

    internal static void QChoiceFilterBuild(
        Panel list,
        IReadOnlyList<string> languages,
        CCatalogFilter filter,
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
                IsChecked = filter.CCatalogFilterMatch(language),
                Content = QChoiceRowBuild(language),
            };
            box.Click += handler;
            list.Children.Add(box);
        }
    }

    internal static void QChoiceKindRefine(
        Panel list, CCatalogFilter filter, RoutedEventHandler handler, IReadOnlyList<CReferenceKind> kinds)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(kinds);

        list.Children.Clear();
        foreach (CReferenceKind kind in kinds)
        {
            CheckBox box = new()
            {
                Style = (Style)list.FindResource("Theme.Choice.Filter"),
                Tag = kind.CReferenceKindTag,
                IsChecked = filter.CCatalogFilterMatch(kind.CReferenceKindTag),
            };
            box.SetResourceReference(ContentControl.ContentProperty, kind.CReferenceKindKey);
            box.Click += handler;
            list.Children.Add(box);
        }
    }

    internal static void QChoiceMenuRefine(Panel list, RoutedEventHandler handler, IReadOnlyList<CReferenceKind> kinds)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(kinds);

        list.Children.Clear();
        foreach (CReferenceKind kind in kinds)
        {
            RadioButton choice = new()
            {
                GroupName = nameof(QChoiceMenuRefine),
                Tag = kind.CReferenceKindTag,
            };
            choice.SetResourceReference(FrameworkElement.StyleProperty, "Theme.Choice.Order");
            choice.SetResourceReference(ContentControl.ContentProperty, kind.CReferenceKindKey);
            choice.Click += handler;
            list.Children.Add(choice);
        }
    }

    internal static void QChoiceMenuApply(Panel list, string tag)
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

    internal static CCatalogFilter QChoiceFilterRead(object sender)
    {
        ArgumentNullException.ThrowIfNull(sender);

        List<string> hidden = [];
        foreach (object child in ((Panel)((FrameworkElement)sender).Parent).Children)
        {
            if (child is CheckBox { IsChecked: not true, Tag: string language })
            {
                hidden.Add(language);
            }
        }

        return new CCatalogFilter(hidden);
    }

    internal static CCatalogOrder? QChoiceOrderRead(object sender)
    {
        return sender is RadioButton { Tag: string word }
            ? QChoiceWord.FirstOrDefault(pair => pair.Value == word).Key
            : null;
    }

    private static string QChoiceKeyRead(string prefix, string word)
    {
        return prefix + "." + word;
    }

    private static void QChoiceWordApply(Popup dropdown, string word)
    {
        foreach (RadioButton button in QChoiceButtonScan(dropdown.Child))
        {
            button.IsChecked = string.Equals(button.Tag as string, word, StringComparison.Ordinal);
        }
    }

    private static Grid QChoiceRowBuild(string language)
    {
        Grid row = new();
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        Grid badge = new() { Width = 19, Height = 14, VerticalAlignment = VerticalAlignment.Center };
        ImageSource? flag = QEnsignImage.QEnsignRead(language);
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

    private static IEnumerable<RadioButton> QChoiceButtonScan(DependencyObject? root)
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

            foreach (RadioButton found in QChoiceButtonScan(node))
            {
                yield return found;
            }
        }
    }
}
