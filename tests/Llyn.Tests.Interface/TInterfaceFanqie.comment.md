# TInterfaceFanqie.cs
Hash: `923e5249f6b1ebca`

## `internal static class TInterfaceFanqie`

The relays for the reads built on fanqie rows and their rimes.
That is the anchor scans, the diwei rime, rank and section reads, and the reading hypothesis.
Each relay is transparent and carries no test logic of its own.
The fanqie row builders stay on `TInterface`, beside the full builder in `TInterfaceFact`.

## `internal static LDiweiSection TDiweiSectionScan(bool respelled, Func<string, string?> localize)`

The diwei section scan is relayed over one fixed row and two tally lines.
Only the respelling switch and the localizer vary.

## `internal static LHypothesis THypothesisCreate(IReadOnlyDictionary<string, string> initials, IReadOnlyDictionary<string, string> finals, IReadOnlyDictionary<string, IReadOnlyList<LHypothesisTone>> tones, IReadOnlyList<LHypothesisLocus>? places = null)`

Builds the hypothesis tables directly, so a test needs no language pack to check the join.
