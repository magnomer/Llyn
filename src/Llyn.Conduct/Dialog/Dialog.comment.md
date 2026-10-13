# Dialog

Dialog holds the gates of two modal windows a driver puts up, the naming and the import customs window.
Both drivers show the same declaration, so the rules live here and not in either medium.
The signatures carry only .NET and Conduct types, so no engine record crosses the cut through them.
It stands apart from the panels because each window has its own short-lived state.

## `CSCoinage`

The naming dialog's gate, shared by every medium that asks for a new tag or register name.
`CSCoinageWordingCheck` refuses blank or whitespace-only wording.

## `CSCustoms`

The customs window's gate: each row's mode and target, the rules over them, and what a Replace would drop.
Rows are addressed by their position in the imported file.
Conduct builds it, hands it to the customs question, then reads the declared rows back itself.
So no driver hands the rows back.

Each imported entry gets one row, listing the ids of the stored entries sharing its headword.
A row with exactly one target starts as Merge into it, and every other row starts as New.
The row keeps its pick across mode changes, so switching back to Merge restores it.
Only a Replace with a chosen target words a loss, with the target's ready card counts.
`CSCustomsReadyCheck` accepts when every Merge or Replace row has a chosen target.
The engine would refuse an intake with no target, so the window refuses it first.
The gates that set a mode or target answer whether the window can now be accepted.
`CSCustomsMode` is mapped to the engine's own mode only when the intakes are sent.
`CSCustomsRow` carries a wording key for the loss and the meaning and collocation cards a Replace drops.

## Stamp

Commit: `c2f335c331fe`
- `CSCoinage.cs` `5acad94c0d048741`
- `CSCustoms.cs` `0de368aca71b171d`
- `CSCustomsMode.cs` `5d62f1204ec0e8a4`
- `CSCustomsRow.cs` `423a98142d4c7c7a`
