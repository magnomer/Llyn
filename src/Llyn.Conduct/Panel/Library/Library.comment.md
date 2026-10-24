# Library

This folder holds the library panel and one side of the duplex panel.
These two panels browse every entry by headword.

## `CLibrary`

The library panel, holding every entry and the panel over their vista.
It finds the rows, while the panel's aperture takes the query, order and language filter.
Its panel loads, edits and deletes the chosen entry, and its own editor edits it.
It is worded under the `Scribe` scope and finishes through the editor.
It prints and exports the chosen entry, and imports a markup file into the workspace.
The rows come already filtered, sorted, twinned and marked, so the list decides nothing about them.
The count goes to the aperture through `CApertureCountSet`, so the empty notice follows the rows shown.
The wings share the library's orderings, so no driver names an engine ordering.
The window's exit gate runs `LLibraryClose`, which stops the editor and cancels the display's playback.
Print opens no window when no entry is chosen.

## `CLibraryEntryCreate`

Opens a fresh entry in the panel and writes each given field through the entry controller.
A null field is left as it is.
It saves through the editor and answers whether the save ended the tenure, as `CEditorEntrySave` does.
A declined leave question opens no fresh entry, so the fields then land on the held draft.

## `CLibraryEntryExport`

Chooses the row by id, then runs the export gate on it.
It answers the path written, or null when the row did not open, the reader cancelled or the export failed.

## `CLibraryMarkupImport`

Asks the envoy for a markup file first, before the customs and omission questions.
It names the title key `Markup.Title` and the one suffix `.llx`.
A cancelled pick answers null and changes nothing.
The engine reads the file, asks for the intakes through this panel, and stores the cargo in one call.
A declined declaration stores nothing, so the rows are not refreshed, nothing is reported and it answers null.
After a store the rows refresh, and what the import could not place is shown through the envoy.
The report is shown only when it holds a line.
A store answers the count of intakes the declaration listed.
Any failure is shown under `List.ImportFailed` and answers null, and the engine stores every entry or none.

## `LLibraryIntakeRead`

Builds the customs gate over the parsed entries and their ready targets, then asks the customs question.
Once accepted, it reads the declared rows back from its own gate and maps each to its engine intake.
A row's place in the answer is its entry's place in the file.
The mode maps one to one, and the engine's factory drops the target of a new entry.
`LLibraryModeRead` throws on a mode it does not know, so a new member cannot slip through as another.
The entry, target and omission reads copy and hold no rule.
The omission's line number is written in the current culture, so a driver only shows it.

## `CWing`

One side of the duplex panel holds an entry vista and its reading display.
It finds the rows, takes the query, order and language sieve, and loads the chosen entry.
It moves the choice through the listed rows for the keys that walk the list.
The chosen entry is saved to the workspace state under the side, so the next run reopens it.
It restores its vista on every workspace change and its entry on every open.
The driver owns the match list, the focus and the lectern that paints the display.
The left flag names its vista's tab and its slot in the workspace state.
The reading display is built here, since a side has no editor to own it.
It is handed the atelier's repaint memory, so both sides and every editor share it.
It starts its first vista at once, so the driver's first paint finds one.
It hears the workspace state itself, so no driver hands an entry id down.

## `CWingChanged`

Raised on each vista, entry, reflex and settings announcement, so the driver re-lists the matches.
`CWingLoaded` is raised once an entry is loaded.
A load that failed raises nothing, since the side stands where it stood.
`CWingRowsChanged` is raised when a key moved the choice, since a selection raises no engine announcement.

## `CWingRowsShown`

Whether the side's match list is open, so no view keeps that flag.
A query held on a vista opens it, and an empty query closes it.
`CWingRowsClose` closes it and answers whether it was open, so a key closes the list once and then passes on.
`CWingRowsOpen` loads the row only while the list is open, and a successful open closes the list.
A closed list, or a refused open, answers false and the list stays as it was.

## `CWingEmpty`

Whether the side shows its empty notice.
That needs a query held and the last rows read found nothing.
An unqueried side is never empty.

## `LWingVistaRestore`

Starts a fresh vista under the side's tab, with entries in headword order and nothing chosen.
It forgets the rows last read, so the keys move nothing until the new rows are read.
A workspace change starts a fresh vista with nothing typed, so the query is not carried over.

## `CWingOrderSet`

Hands a chosen ordering to the vista, which saves it under the side's tab and announces it.
A sender that is no order row hands null, which keeps the ordering it has.

## `CWingRowMove`

Moves the vista's choice one listed row down or up, without loading or saving.
The move walks the rows last read, which are the rows the driver shows.
It stops at either end, and no chosen row counts as the place before the first.
So Down and Up both land on the first row when nothing is chosen.
An empty list answers null and raises nothing, so the key stays with the field.
A closed list answers null too, so the keys move nothing while no list shows.

## `LWingEntryRestore`

Answers every workspace open with the entry the state names for this side.
The shown entry closes first, so a failed load never leaves another workspace's entry standing.
An empty slot loads nothing.
The restore saves nothing, since the workspace state already names that entry.

## `CWingEntryOpen`

Loads the entry picked from the list and saves the side's standing.
No row picks nothing and answers false, so the key stays with the field and the list stays open.

## `CWingRowsRead`

The rows the engine returns for the vista, crossing as shapes so the side names no engine row.
The side holds them, so the keys and the empty notice follow what the driver shows.
A failed find is shown under `Duplex.LoadFailed` and answers no rows.
So a swapped engine that throws never reaches the driver, and the side holds no stale rows.

## `LWingEntryLoad`

An entry that is gone leaves the side empty rather than showing what it was.
A load that failed leaves the side where it stood and reports the failure through the envoy.
The side only passes the draft across, so it never keeps it.
`LWingClose` stops the display's playback when the atelier closes.

## Stamp

Commit: `78fc0ebfff7c`
- `CLibrary.cs` `dfed08c8f774c7f4`
- `CWing.cs` `1e096be3dc635adc`
