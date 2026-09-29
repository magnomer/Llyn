# TPanel.cs

## `public sealed class TPanel`

Covers the browsing panel's gates end to end on a real workspace.
Leaving the scribe with unstored changes asks the envoy once.
Keeping stays in the scribe, storing finishes the draft, and discarding shows the viewer unfinished.
A clean editor never asks, and entering the scribe hands the chosen row to the editor.
The fresh gate drops the chosen row and opens the scribe on nothing.
A refused leave keeps the row the panel stands on.
A row click asks the leave question before it records the station.
A declined leave records no station and opens nothing.
Opening a stored row chooses it, and in edit mode hands it to the editor.
Opening a row that no longer loads closes the panel without a failure notice.
A quiet reload repaints a stored entry and closes the panel on a deleted one.
A stored-entry notice re-lists the rows and repaints the mode once each.
The delete gate asks through the envoy, keeps everything on a refusal and drops the row on a yes.
A panel with no delete scope never deletes, and a referenced record is asked about with its tally.
A stored entry is adopted only by a panel editing on nothing.
Restoring the scribe needs a row, and a restored vista opens in the viewer.
The order, subject and filter maps carry every member by name.
These facts moved here from the Windows suite with the panel.

## `private static CPanel TPanelPrepare(LEngine engine, bool? answer, List<string> asked, List<bool> finished, string? scope, Func<bool> change)`

Builds a panel over a library vista standing on one stored entry.
The envoy answers `answer` and records each question in `asked`.
The finish seam records each store in `finished`, and `change` says whether the editor holds changes.
