# TInterfaceFanqie.cs
Hash: `9835e3d194d52aa7`

## `internal static class TInterfaceFanqie`

The relays for the reads built on fanqie rows and their rimes.
That is the anchor scans, the diwei rime, rank and section reads, the tally marks, and the reading hypothesis.
Each relay is transparent and carries no test logic of its own.
The fanqie row builders stay on `TInterface`, beside the full builder in `TInterfaceFact`.

## `internal static LDiweiSection TDiweiSectionScan(bool respelled, Func<string, string?> localize)`

The diwei section scan is relayed over one fixed row and two tally lines.
Only the respelling switch and the localizer vary.

## `internal static IReadOnlyList<LDiweiSection> TDiweiSectionScan(IReadOnlyList<LFanqieRow> rows)`

The diwei section scan is relayed over the given rows of one initial page, with no tallies.
It returns every section, so a test reads both the section order and how one line lists its characters.

## `internal static LHypothesis THypothesisCreate(IReadOnlyDictionary<string, string> initials, IReadOnlyDictionary<string, string> finals, IReadOnlyDictionary<string, IReadOnlyList<LHypothesisTone>> tones, IReadOnlyList<LHypothesisLocus>? places = null)`

Builds the hypothesis tables directly, so a test needs no language pack to check the join.

## `internal static IReadOnlyList<LTallyMark> TTallyMarkScan(IReadOnlyDictionary<string, List<string>> parts)`

Relays the tally mark scan, so a test pins how one mark lists its characters.
