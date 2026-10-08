# CDiptych.cs
Hash: `a0f45daf855a9029`

## `public sealed class CDiptych`

The routing between a parent list and the child list whose rows follow the parent's chosen row.
The corpus, the repertoire, the shelf and the guild share it, so their two-sided mode lives in one place.
The two panels' chosen rows and editing flags are the mode, and the drivers only follow.
The child side is in front exactly when the child list has a chosen row or is in edit mode.
It asks the leave question through the owner's session, which asks the user through the envoy.
Seams stand in for the owner's desk start, desk cancel and child create, which differ per panel.

## `internal CDiptych(CPanel parent, CPanel child, CSession session, CNavigation station, Action<long?> startSeam, Action cancelSeam, Action? createSeam)`

Holds both panels, the owner's session and the navigation that records a voyage.
`startSeam` starts the owner's desk, and `cancelSeam` cancels it.
`createSeam` opens a fresh child entry for the chosen row.
It may be null, and then a create always starts a blank parent entry.
Building it is no user action, so the owner builds it right after its session.

## `public bool CDiptychChildSide`

True while the child side is in front.
The mode flags below follow it and whether the side in front edits.

## `public bool CDiptychParentEditing`

True while the parent side is in front and in edit mode.

## `public bool CDiptychParentShown`

True while the parent side is in front outside edit mode.

## `public bool CDiptychChildEditing`

True while the child list is in edit mode.

## `public bool CDiptychChildShown`

True while the child side is in front outside edit mode.

## `public bool CDiptychScribeChecked`

True while either side edits.

## `public bool CDiptychModeEnabled`

Holds on the child side, and on the parent side follows the parent panel.

## `public bool CDiptychBinEnabled`

Holds only on the parent side, where it follows the parent panel.

## `public void CDiptychParentSelect(long? id)`

Shows the clicked parent row once the user agrees to leave unsaved changes.
The mode is read before the question, because a save from the dialog must not drop the scribe.
The voyage is recorded only after the user agreed, so a refused leave keeps the Forward history.
A save from the dialog closes the desk through the session, so the row loads once.

## `internal void LDiptychParentShow(long id, bool editing)`

Clears the child side and shows the parent row in the mode the caller read.
The parent panel loads the row, clears a vanished one and restarts the owner's desk through its edit event.
The owner's arrival from the navigation shows its row through it.

## `public void CDiptychChildSelect(long? id)`

Shows the chosen child row once the user agrees to leave unsaved changes.
The mode is read before the question, as `CDiptychParentSelect` does.
The child row loads first, and nothing else changes when it fails or has vanished.
Only a held child row cancels the owner's desk and closes the parent scribe.
The scribe then reopens on the child row when either side was editing.

## `public void CDiptychEntryCreate()`

Opens a child entry through the create seam while either list has a chosen row.
Otherwise, or without a create seam, it starts a blank parent entry.
Both ask the leave question first.

## `public void CDiptychScribeToggle(bool editing)`

Toggles the scribe on the side in front.
Closing the child side falls back to the chosen parent row, or clears both lists without one.
Closing the parent scribe cancels the owner's desk.

## `internal void LDiptychEntryClose()`

Clears both lists.
The owner calls it when the workspace changes and when its rows no longer list the shown row.

## `internal void LDiptychChildResonate()`

Answers the chosen child row's notice by refreshing the child draft.
The child list's observers call it through the owner's marshal, so no driver hands the notice over.
It does nothing without a chosen child row, so a whole-set bulletin never clears a fresh entry.
The mode is read before the refresh, so a vanished child row under edit reopens the parent scribe.
It falls back to the chosen parent row once the child side has closed.

## `public void CDiptychEntryDelete()`

Deletes the chosen parent row through the parent panel's bin, which asks the envoy first.
It does nothing on the child side, whose panel has no delete scope.
