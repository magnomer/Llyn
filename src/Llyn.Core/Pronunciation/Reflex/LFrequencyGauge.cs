using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Llyn.Core;

public sealed record LFrequencyGauge(int LFrequencyGaugeBand, string LFrequencyGaugeSource)
{
    public static LFrequencyGauge? LFrequencyGaugeResolve(IReadOnlyList<LFrequency> rows, string once)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(once);

        if (rows.Count == 0)
        {
            return null;
        }

        int band = 0;
        foreach (LFrequency row in rows)
        {
            if (row.LFrequencyRank > 0)
            {
                band = row.LFrequencyRank;
                break;
            }
        }

        return new LFrequencyGauge(band, LFrequencyGaugeFormat(rows, once));
    }

    private static string LFrequencyGaugeFormat(IReadOnlyList<LFrequency> rows, string once)
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
}
