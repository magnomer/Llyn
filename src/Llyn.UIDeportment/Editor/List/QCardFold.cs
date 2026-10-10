using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Llyn.UIDeportment;

internal static class QCardFold
{
    internal static void QCardFoldRefine(FrameworkElement container, bool folded, bool stored)
    {
        if (QLook.QLookPartFind<ToggleButton>(container, "PCardHinge") is ToggleButton hinge)
        {
            hinge.IsChecked = folded;
            hinge.Visibility = QLook.QLookVisibleRead(stored);
        }

        if (QLook.QLookPartFind<Border>(container, "PCardHeader") is Border header)
        {
            if (folded)
            {
                header.SetResourceReference(Border.CornerRadiusProperty, "Theme.Card.FoldRadius");
                header.SetResourceReference(Border.BorderThicknessProperty, "Theme.Card.FoldEdge");
            }
            else
            {
                header.ClearValue(Border.CornerRadiusProperty);
                header.ClearValue(Border.BorderThicknessProperty);
            }
        }

        if (QLook.QLookPartFind<Grid>(container, "PCardBody") is Grid body)
        {
            body.Visibility = QLook.QLookVisibleRead(!folded);
        }
    }
}
