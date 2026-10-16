using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Llyn.Core;

public sealed record LDeckCensus(
    int LDeckCensusTotal,
    int LDeckCensusActive,
    int LDeckCensusExcluded,
    int LDeckCensusOrphaned,
    int LDeckCensusFlagged,
    DateTimeOffset? LDeckCensusSynced,
    IReadOnlyList<LDeckWeek> LDeckCensusWeeks)
{
    public const int LDeckCensusSpan = 12;

    public static LDeckCensus LDeckCensusBuild(
        IReadOnlyList<LCue> cues, IReadOnlyList<string?> synced, DateTimeOffset now, TimeZoneInfo zone)
    {
        ArgumentNullException.ThrowIfNull(cues);
        ArgumentNullException.ThrowIfNull(synced);
        ArgumentNullException.ThrowIfNull(zone);

        DateTimeOffset? latest = null;
        foreach (string? stamp in synced)
        {
            if (stamp is not null
                && DateTimeOffset.TryParse(
                    stamp, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset moment)
                && (latest is null || moment > latest))
            {
                latest = moment;
            }
        }

        return new LDeckCensus(
            cues.Count,
            cues.Count(static cue => !cue.LCueResting),
            cues.Count(static cue => cue.LCueExcluded),
            cues.Count(static cue => cue.LCueOrphaned),
            cues.Count(static cue => cue.LCueFlag != 0),
            latest,
            LDeckWeek.LDeckWeekDivide(
                [.. cues.Select(static cue => cue.LCueAddedUtc)], now, zone, LDeckCensusSpan));
    }
}
