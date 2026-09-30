using System;
using System.Windows;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal static class QStateConverter
{
    internal static void QStateTextRefine(DependencyObject target, DependencyProperty property, CStateWording wording)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(wording);

        target.SetValue(
            property,
            wording.CStateWordingKey is string key
                ? QLocalizationCatalog.QLocalizationTextRead(key)
                : wording.CStateWordingText);
    }

    internal static void QStateHintRefine(DependencyObject target, DependencyProperty property, CStateWording wording)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(wording);

        target.SetValue(
            property,
            wording.CStateWordingHint is string hint
                ? QLocalizationCatalog.QLocalizationTextRead(hint)
                : string.Empty);
    }
}
