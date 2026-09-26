# CSCustoms.cs

## `public sealed class CSCustoms`

The customs window's gate: each row's mode and target, the rules over them, and what a Replace would drop.
Both drivers show the same declaration, so the rules live here and not in either medium.
Rows are addressed by their position in the imported file.
Its signatures carry only .NET and Conduct types, so no engine record crosses the cut through it.

## `public CSCustoms(IReadOnlyList<IReadOnlyList<long>> candidates)`

One row per imported entry, each listing the ids of the stored entries sharing its headword.
A row with exactly one candidate starts as Merge into it, and every other row starts as New.

## `public CSCustomsRow CSCustomsRowRead(int row)`

Everything a driver shows for one row, so one call fills it.
The row keeps its pick across mode changes, so switching back to Merge restores it.
Only a Replace with a chosen target names an entry whose cards are lost.

## `public bool CSCustomsModeSet(int row, CSCustomsMode mode)`

Takes the user's mode for one row and answers whether the window can now be accepted.

## `public bool CSCustomsTargetSet(int row, long target)`

Takes the user's target for one row and answers whether the window can now be accepted.

## `public bool CSCustomsReadyCheck()`

Whether the window can be accepted: every New row, and every other row once a target is chosen.
The engine would refuse an intake with no target, so the window refuses it first.

## `public static int CSCustomsCardScan<CSCustomsCard>(`

Counts the cards and every card nested under them.
Nested cards are counted too, since every one of them goes.
The card type is a parameter, so the gate counts the driver's cards without naming an engine record.
