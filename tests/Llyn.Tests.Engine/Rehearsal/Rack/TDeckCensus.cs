using System;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TDeckCensus
{
    private static readonly DateTimeOffset TDeckCensusNow = new(2026, 10, 14, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void DeckCensusBuild_MixedCues_CountsEachState()
    {
        LCue free = TInterface.TCueCreate(LGrain.LGrainFree, null, null, null, null, null);
        LCue orphan = TInterface.TCueCreate(LGrain.LGrainMeaning, null, null, null, null, null);

        LDeckCensus census = TInterface.TDeckCensusBuild(
            [
                free,
                free with { LCueFlag = 2 },
                free with { LCueExcluded = true },
                orphan,
                orphan with { LCueExcluded = true },
            ],
            [null],
            TDeckCensusNow,
            TimeZoneInfo.Utc);

        Assert.Equal(5, census.LDeckCensusTotal);
        Assert.Equal(2, census.LDeckCensusActive);
        Assert.Equal(2, census.LDeckCensusExcluded);
        Assert.Equal(2, census.LDeckCensusOrphaned);
        Assert.Equal(1, census.LDeckCensusFlagged);
        Assert.Equal(LDeckCensus.LDeckCensusSpan, census.LDeckCensusWeeks.Count);
    }

    [Fact]
    public void DeckCensusBuild_SeveralSyncs_ReturnsLatest()
    {
        LDeckCensus census = TInterface.TDeckCensusBuild(
            [],
            [null, "2026-10-03 07:00:00", "2026-10-01 08:00:00", "not a time"],
            TDeckCensusNow,
            TimeZoneInfo.Utc);

        Assert.Equal(new DateTimeOffset(2026, 10, 3, 7, 0, 0, TimeSpan.Zero), census.LDeckCensusSynced);
    }

    [Fact]
    public void DeckCensusBuild_NeverSynced_ReturnsNoSync()
    {
        LDeckCensus census = TInterface.TDeckCensusBuild([], [null], TDeckCensusNow, TimeZoneInfo.Utc);

        Assert.Null(census.LDeckCensusSynced);
        Assert.Equal(0, census.LDeckCensusTotal);
    }
}
