# PRepertoireEditor.cs

## `public partial class PRepertoire`

The edit area of the Repertoire panel, `PScenario`, and what its three fields do.
Typing defers a save, and the save sends one request per field that differs from what the engine holds.
The engine answers with a bulletin, and the fields are redrawn from the draft only where they differ.
So a keystroke never loses the caret to its own echo.

## Inline notes

### `private bool _pScenarioLoading;`

Set while fields are being filled from a held Situation.
Filling a field raises the same change the user typing raises, and only the second may clear an unknown mark.

## `private string PScenarioHintRead(TextBox field, bool unknown)`

The placeholder one field shows while it is empty.
An unknown field shows the unknown mark, and a never-written one asks for what it wants.
The title asks with the untitled text the catalog uses, so an empty title reads the same in both places.

## `private void PScenarioApply(LSituation? situation)`

Fills the three fields outright from a Situation, or empties them, when a draft is opened or closed.
The picture and video rows are filled with them, so an opened draft shows every row it holds.

## `private void PScenarioShow(LSituation situation)`

Redraws each field from the held Situation only where the field says something else.
A field that already reads what the engine holds is left alone, caret included.
The picture and video rows follow the same rule, row by row, through `PRepertoireDialog.cs`.

## `private LRequestSituationBody PScenarioRead(long draft)`

The editor as written, for the engine to read into the Situation it holds.
Each field travels with its mark, so the panel resolves no state.
The body names no situation id, since the engine lands it on the Situation the draft holds.

## `private void PScenarioStoreRun()`

Finishes the desk over the held Situation, for the rail's save button.
It stores over the selected one or creates one nothing references yet.
Typing still waiting to be written is written first, so the commit carries the last keystroke.
An unchanged draft is not committed, since storing what matches its Situation would rewrite it for nothing.
Whether this is a create or a rewrite is the engine's reading of the draft, not the panel's.
Saving does not change the id, what references it, or the order it takes for any referrer.
A halted tenure refuses to finish, so a draft missing the edits a failed flush dropped stores nothing.


## `private void PScenarioStoredShow(long situation)`

Selects and shows the Situation the desk just stored, back in the reading side.
The desk announces the id on every finish that stored, the rail's save and the window's leave alike.
