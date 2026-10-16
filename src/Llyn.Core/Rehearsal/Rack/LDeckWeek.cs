using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

public sealed record LDeckWeek(DateOnly LDeckWeekMonday, int LDeckWeekAdded)
{
    public static IReadOnlyList<LDeckWeek> LDeckWeekDivide(
        IReadOnlyList<string> added, DateTimeOffset now, TimeZoneInfo zone, int span)
    {
        ArgumentNullException.ThrowIfNull(added);
        ArgumentNullException.ThrowIfNull(zone);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(span);

        Func<DateTimeOffset, DateOnly> monday = moment =>
        {
            DateOnly day = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(moment, zone).DateTime);
            return day.AddDays(-(((int)day.DayOfWeek + 6) % 7));
        };

        DateOnly first = monday(now).AddDays(-7 * (span - 1));
        int[] counts = new int[span];
        foreach (string stamp in added)
        {
            if (!DateTimeOffset.TryParse(
                    stamp, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset moment))
            {
                continue;
            }

            int index = (monday(moment).DayNumber - first.DayNumber) / 7;
            if (index >= 0 && index < span)
            {
                counts[index]++;
            }
        }

        List<LDeckWeek> weeks = [];
        for (int index = 0; index < span; index++)
        {
            weeks.Add(new LDeckWeek(first.AddDays(7 * index), counts[index]));
        }

        return weeks;
    }
}
