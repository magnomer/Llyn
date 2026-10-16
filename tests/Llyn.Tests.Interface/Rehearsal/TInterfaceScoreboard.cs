using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LDeckCensus TDeckCensusBuild(
        IReadOnlyList<LCue> cues, IReadOnlyList<string?> synced, DateTimeOffset now, TimeZoneInfo zone) =>
        LDeckCensus.LDeckCensusBuild(cues, synced, now, zone);

    internal static IReadOnlyList<LDeckWeek> TDeckWeekDivide(
        IReadOnlyList<string> added, DateTimeOffset now, TimeZoneInfo zone, int span) =>
        LDeckWeek.LDeckWeekDivide(added, now, zone, span);

    internal static LDeckCensus TCueCensusRead(this LCueClerk clerk, long? deckId, TimeZoneInfo zone) =>
        clerk.LCueCensusRead(deckId, zone);

    internal static LDeckCensus TEngineCensusRead(this LDeckPort port, long? deckId) =>
        port.LEngineCensusRead(deckId);
}
