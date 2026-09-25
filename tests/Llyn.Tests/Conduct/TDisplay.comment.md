# TDisplay.cs

## `public sealed class TDisplay`

Covers the reading view rules that moved into the conduct, driven with no window.

## `public void DisplayStampFormat_UnreadableText_ReturnsEmpty()`

A stamp that does not parse, or none at all, shows as nothing.

## `public void DisplayFoldScan_NoRule_ReturnsEmpty()`

No reflex rule folds no language.

## `public void DisplayFoldSet_CurrentValue_KeepsFold()`

Setting the fold to the value it already holds leaves it there, so a toggle echo changes nothing.

## `public void DisplayBandResolve_UnbandedRows_ReturnsZero()`

Rows that carry no band resolve to the unknown band.

## `public void DisplaySourceFormat_OnceInterval_FormatsInterval()`

A row with a once interval shows the interval in the given pattern, and a plain row shows its figure.
Each source takes its own line.
