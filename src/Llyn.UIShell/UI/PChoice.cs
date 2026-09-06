using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
