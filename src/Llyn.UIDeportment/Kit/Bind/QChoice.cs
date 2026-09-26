using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal static class QChoice
{
    internal static void QChoiceOrderBuild(
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
                Tag = LCatalog.LCatalogOrderFormat(order),
            };
            choice.SetResourceReference(FrameworkElement.StyleProperty, "Theme.Choice.Order");
            choice.SetResourceReference(
                ContentControl.ContentProperty, QChoiceKeyRead(prefix, LCatalog.LCatalogOrderFormat(order)));
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

    internal static void QChoiceOrderApply(Popup dropdown, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(dropdown);

        QChoiceWordApply(dropdown, LCatalog.LCatalogOrderFormat(order));
    }

    internal static void QChoiceFilterBuild(
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
                Content = QChoiceRowBuild(language),
            };
            box.Click += handler;
            list.Children.Add(box);
        }
    }

    internal static void QChoiceKindBuild(Panel list, LCatalogFilter filter, RoutedEventHandler handler)
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

    internal static void QChoiceMenuBuild(Panel list, RoutedEventHandler handler, IReadOnlyList<LReferenceKind> kinds)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(kinds);

        list.Children.Clear();
        foreach (LReferenceKind kind in kinds)
        {
            RadioButton choice = new()
            {
                GroupName = nameof(QChoiceMenuBuild),
                Tag = LReference.LReferenceKindFormat(kind),
            };
            choice.SetResourceReference(FrameworkElement.StyleProperty, "Theme.Choice.Order");
            choice.SetResourceReference(ContentControl.ContentProperty, LReference.LReferenceKindResolve(kind));
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

    internal static LCatalogFilter QChoiceFilterRead(Panel list)
    {
        ArgumentNullException.ThrowIfNull(list);

        List<string> hidden = [];
        foreach (object child in list.Children)
        {
            if (child is CheckBox { IsChecked: not true, Tag: string language })
            {
                hidden.Add(language);
            }
        }

        return hidden.Count == 0 ? LCatalogFilter.LCatalogFilterEmpty : new LCatalogFilter(hidden);
    }

    internal static LCatalogOrder? QChoiceOrderRead(object sender)
    {
        return sender is RadioButton { Tag: string word }
            ? LCatalog.LCatalogOrderParse(word, LCatalogOrder.LCatalogOrderName)
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
