# TPanel.cs
Hash: `4be8b881c1cb5166`

## `public sealed class TPanel`

Covers the browsing panel's scribe gates end to end on a real workspace.
Leaving the scribe with unstored changes asks the envoy once.
Keeping stays in the scribe, storing finishes the draft, and discarding shows the viewer unfinished.
A clean editor never asks, and entering the scribe hands the chosen row to the editor.
The fresh gate drops the chosen row and opens the scribe on nothing.
Restoring the scribe needs a row, and a restored vista opens in the viewer.
The order and subject maps carry every member by name, and the filter map carries its hidden languages.
Row choice and stored-entry notices live in `TPanelRow`, and the delete gate in `TPanelBin`.

## `internal static CPanel TPanelPrepare(LEngine engine, bool? answer, List<string> asked, List<bool> finished, string? scope, Func<bool> change)`

Builds a panel over a library vista standing on one stored entry.
The envoy answers `answer` and records each question in `asked`.
The finish seam records each store in `finished`, and `change` says whether the editor holds changes.
`TPanelRow` and `TPanelBin` build their panel through it.
