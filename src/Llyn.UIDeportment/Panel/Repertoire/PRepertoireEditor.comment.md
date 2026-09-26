# PRepertoireEditor.cs

## `public partial class PRepertoire`

The edit area of the Repertoire panel, `PScenario`, and what its three fields do.
Typing defers a save, and the save sends one whole-form `LRequestSituationBody` through `PScenarioChangeDefer`.
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

## `private void PScenarioHintApply()`

Feeds the two unseen twins of the title, the work their bindings did before.
The hint copies the placeholder and shows only while the title is empty, so the head row closes on it.
The ghost copies the written title, so the head row closes on the text.
It runs after the title or its placeholder changes.

## `private void PScenarioMeasureApply()`

Feeds the unseen twin of the kind with the written kind, or with the placeholder while the kind is empty.
The chip closes on that text, as its binding and trigger made it do before.

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
