using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public static class LFrequencyLabel
{
    private const int LFrequencyLimit = 4;

    private const string LFrequencyEmpty = "Empty";

    private const char LFrequencyStar = '✦';

    public static void LFrequencyChipShow(
        UIElement section, FrameworkElement chip, TextBlock name, TextBlock band, IReadOnlyList<LFrequency> rows)
    {
        if (!LDisplay.LDisplayFrequencyCheck(rows))
        {
            name.Text = string.Empty;
            chip.ToolTip = null;
            section.Visibility = Visibility.Collapsed;
            return;
        }

        LFrequencyLabelShow(name, band, LDisplay.LDisplayBandResolve(rows));
        chip.ToolTip = LDisplay.LDisplaySourceFormat(
            rows, LLocalizationCatalog.LLocalizationTextRead("Frequency.Once"));
        section.Visibility = Visibility.Visible;
    }

    private static void LFrequencyLabelShow(TextBlock name, TextBlock band, int count)
    {
        string brush = LDisplay.LDisplayBandRead(count, "Theme.Frequency.");
        name.Text = LLocalizationCatalog.LLocalizationTextRead(LDisplay.LDisplayBandRead(count, "Frequency."));
        name.SetResourceReference(TextBlock.ForegroundProperty, brush);

        band.Inlines.Clear();
        if (count == 0)
        {
            band.Visibility = Visibility.Collapsed;
            return;
        }

        Run filled = new(new string(LFrequencyStar, count));
        filled.SetResourceReference(TextElement.ForegroundProperty, brush);
        band.Inlines.Add(filled);

        int rest = LFrequencyLimit - count;
        if (rest > 0)
        {
            Run empty = new(new string(LFrequencyStar, rest));
            empty.SetResourceReference(TextElement.ForegroundProperty, "Theme.Frequency." + LFrequencyEmpty);
            band.Inlines.Add(empty);
        }

        band.Visibility = Visibility.Visible;
    }
}
