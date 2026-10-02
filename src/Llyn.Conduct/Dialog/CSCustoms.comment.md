# CSCustoms.cs
Hash: `0de368aca71b171d`

## `public sealed class CSCustoms`

The customs window's gate: each row's mode and target, the rules over them, and what a Replace would drop.
Both drivers show the same declaration, so the rules live here and not in either medium.
Rows are addressed by their position in the imported file.
Its signatures carry only .NET and Conduct types, so no engine record crosses the cut through it.
Conduct builds it and hands it to the customs question, then reads the declared rows back itself.

## `internal CSCustoms(IReadOnlyList<CMarkupEntry> entries, IReadOnlyList<CMarkupTarget> targets)`

One row per imported entry, each listing the ids of the stored entries sharing its headword.
A row with exactly one target starts as Merge into it, and every other row starts as New.
`targets` holds every stored entry the rows may join, with the card counts a Replace drops.

## `public IReadOnlyList<CMarkupEntry> CSCustomsEntry`

The entries the window lists, in file order, each with its ready target ids.

## `public CSCustomsRow CSCustomsRowRead(int row)`

Everything a driver shows for one row, so one call fills it.
The row keeps its pick across mode changes, so switching back to Merge restores it.
Only a Replace with a chosen target words a loss, with the target's ready card counts.

## `internal IReadOnlyList<CSCustomsRow> LSCustomsRowsRead()`

Every row in file order, read once the user accepts the window.
The library maps them to the engine's intakes, so no driver hands the rows back.

## `public bool CSCustomsModeSet(int row, CSCustomsMode mode)`

Takes the user's mode for one row and answers whether the window can now be accepted.

## `public bool CSCustomsTargetSet(int row, long target)`

Takes the user's target for one row and answers whether the window can now be accepted.

## `public bool CSCustomsReadyCheck()`

Whether the window can be accepted: every New row, and every other row once a target is chosen.
The engine would refuse an intake with no target, so the window refuses it first.
