using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Core;

namespace Llyn.UIDeportment;

public static class LFrequencyLabel
{
    private static readonly string[] LFrequencyBands = ["Rare", "Advanced", "Everyday", "Core"];

    private const string LFrequencyUnknown = "Unknown";

    private const string LFrequencyEmpty = "Empty";

    private const char LFrequencyStar = '✦';

    public static int LFrequencyBandResolve(IReadOnlyList<LFrequency> rows)
    {
        foreach (LFrequency row in rows)
        {
            if (row.LFrequencyBand is not null)
            {
                return row.LFrequencyRank;
            }
        }

        return 0;
    }

    public static string LFrequencyLabelResolve(int count)
    {
        return "Frequency." + LFrequencyBandRead(count);
    }

    public static void LFrequencyLabelShow(TextBlock name, TextBlock band, int count, string label)
    {
        string brush = "Theme.Frequency." + LFrequencyBandRead(count);
        name.Text = label;
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

        int rest = LFrequencyBands.Length - count;
        if (rest > 0)
        {
            Run empty = new(new string(LFrequencyStar, rest));
            empty.SetResourceReference(TextElement.ForegroundProperty, "Theme.Frequency." + LFrequencyEmpty);
            band.Inlines.Add(empty);
        }

        band.Visibility = Visibility.Visible;
    }

    public static void LFrequencyChipShow(
        UIElement section, FrameworkElement chip, TextBlock name, TextBlock band, IReadOnlyList<LFrequency> rows)
    {
        if (!LFrequency.LFrequencyCheck(rows))
        {
            name.Text = string.Empty;
            chip.ToolTip = null;
            section.Visibility = Visibility.Collapsed;
            return;
        }

        int count = LFrequencyBandResolve(rows);
        LFrequencyLabelShow(
            name, band, count, LLocalizationCatalog.LLocalizationTextRead(LFrequencyLabelResolve(count)));
        chip.ToolTip = LFrequencySourceFormat(rows, LLocalizationCatalog.LLocalizationTextRead("Frequency.Once"));
        section.Visibility = Visibility.Visible;
    }

    public static string LFrequencySourceFormat(IReadOnlyList<LFrequency> rows, string once)
    {
        StringBuilder lines = new();
        foreach (LFrequency row in rows)
        {
            if (lines.Length > 0)
            {
                lines.Append('\n');
            }

            string figure = row.LFrequencyOnce is long interval
                ? string.Format(CultureInfo.CurrentCulture, once, interval.ToString("N0", CultureInfo.CurrentCulture))
                : row.LFrequencyFigure;
            lines.Append(row.LFrequencySource).Append(": ").Append(figure);
        }

        return lines.ToString();
    }

    private static string LFrequencyBandRead(int count)
    {
        return count == 0 ? LFrequencyUnknown : LFrequencyBands[count - 1];
    }
}
