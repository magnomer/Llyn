using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TDeckWeek
{
    private static readonly DateTimeOffset TDeckWeekNow = new(2026, 10, 14, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void DeckWeekDivide_NowMidweek_ReturnsMondayStarts()
    {
        IReadOnlyList<LDeckWeek> weeks = TInterface.TDeckWeekDivide([], TDeckWeekNow, TimeZoneInfo.Utc, 12);

        Assert.Equal(12, weeks.Count);
        Assert.Equal(new DateOnly(2026, 7, 27), weeks[0].LDeckWeekMonday);
        Assert.Equal(new DateOnly(2026, 10, 12), weeks[^1].LDeckWeekMonday);
        Assert.All(weeks, static week => Assert.Equal(DayOfWeek.Monday, week.LDeckWeekMonday.DayOfWeek));
        Assert.All(weeks, static week => Assert.Equal(0, week.LDeckWeekAdded));
    }

    [Fact]
    public void DeckWeekDivide_MondayMidnight_CountsNewWeek()
    {
        IReadOnlyList<LDeckWeek> weeks = TInterface.TDeckWeekDivide(
            ["2026-10-12T00:00:00.0000000Z"], TDeckWeekNow, TimeZoneInfo.Utc, 12);

        Assert.Equal(1, weeks[^1].LDeckWeekAdded);
        Assert.Equal(0, weeks[^2].LDeckWeekAdded);
    }

    [Fact]
    public void DeckWeekDivide_SundayLastSecond_CountsPriorWeek()
    {
        IReadOnlyList<LDeckWeek> weeks = TInterface.TDeckWeekDivide(
            ["2026-10-11 23:59:59"], TDeckWeekNow, TimeZoneInfo.Utc, 12);

        Assert.Equal(0, weeks[^1].LDeckWeekAdded);
        Assert.Equal(1, weeks[^2].LDeckWeekAdded);
    }

    [Fact]
    public void DeckWeekDivide_ZoneAheadOfUtc_CountsLocalMonday()
    {
        TimeZoneInfo zone = TimeZoneInfo.CreateCustomTimeZone(
            "Plus Nine", TimeSpan.FromHours(9), "Plus Nine", "Plus Nine");

        IReadOnlyList<LDeckWeek> weeks = TInterface.TDeckWeekDivide(
            ["2026-10-11 20:00:00"], new DateTimeOffset(2026, 10, 11, 20, 0, 0, TimeSpan.Zero), zone, 12);

        Assert.Equal(new DateOnly(2026, 10, 12), weeks[^1].LDeckWeekMonday);
        Assert.Equal(1, weeks[^1].LDeckWeekAdded);
    }

    [Fact]
    public void DeckWeekDivide_StampOutsideSpan_DropsStamp()
    {
        IReadOnlyList<LDeckWeek> weeks = TInterface.TDeckWeekDivide(
            ["2026-07-26 23:59:59", "2026-07-27 00:00:00", "2026-10-19 00:00:00", "not a time"],
            TDeckWeekNow,
            TimeZoneInfo.Utc,
            12);

        Assert.Equal(1, weeks.Sum(static week => week.LDeckWeekAdded));
        Assert.Equal(1, weeks[0].LDeckWeekAdded);
    }
}
