using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal static class PFrequencyLabel
{
    private static readonly string[] PFrequencyBands = ["Rare", "Advanced", "Everyday", "Core"];

    private const string PFrequencyUnknown = "Unknown";

    private const string PFrequencyEmpty = "Empty";

    private const char PFrequencyStar = '✦';

    internal static int PFrequencyBandResolve(IReadOnlyList<LFrequency> rows)
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

    internal static string PFrequencyLabelResolve(int count)
    {
        return "Frequency." + PFrequencyBandRead(count);
    }

    internal static void PFrequencyLabelShow(TextBlock name, TextBlock band, int count, string label)
    {
        string brush = "Theme.Frequency." + PFrequencyBandRead(count);
        name.Text = label;
        name.SetResourceReference(TextBlock.ForegroundProperty, brush);

        band.Inlines.Clear();
        if (count == 0)
        {
            band.Visibility = Visibility.Collapsed;
            return;
        }

        Run filled = new(new string(PFrequencyStar, count));
        filled.SetResourceReference(TextElement.ForegroundProperty, brush);
        band.Inlines.Add(filled);

        int rest = PFrequencyBands.Length - count;
        if (rest > 0)
        {
            Run empty = new(new string(PFrequencyStar, rest));
            empty.SetResourceReference(TextElement.ForegroundProperty, "Theme.Frequency." + PFrequencyEmpty);
            band.Inlines.Add(empty);
        }

        band.Visibility = Visibility.Visible;
    }

    internal static string PFrequencySourceFormat(IReadOnlyList<LFrequency> rows, string once)
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

    private static string PFrequencyBandRead(int count)
    {
        return count == 0 ? PFrequencyUnknown : PFrequencyBands[count - 1];
    }
}
