using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using Llyn.Core;

namespace Llyn.UIShell;

internal static class PChoice
{
    internal static void PChoiceOrderApply(Popup dropdown, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(dropdown);

        string tag = LCatalog.LCatalogOrderFormat(order);

        foreach (RadioButton button in PChoiceButtonScan(dropdown.Child))
        {
            button.IsChecked = string.Equals(button.Tag as string, tag, StringComparison.Ordinal);
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

    internal static LCatalogFilter PChoiceFilterRead(Panel list)
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

    private static Grid PChoiceRowBuild(string language)
    {
        Grid row = new();
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        Grid badge = new() { Width = 19, Height = 14, VerticalAlignment = VerticalAlignment.Center };
        ImageSource? flag = PEnsign.PEnsignFind(language);
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
