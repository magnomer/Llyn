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

## `private void PScenarioApply(LSituation? situation)`

Fills the three fields outright from a Situation, or empties them, when a draft is opened or closed.

## `private void PScenarioShow(LSituation situation)`

Redraws each field from the held Situation only where the field says something else.
A field that already reads what the engine holds is left alone, caret included.

## `private LSituation PScenarioRead()`

What the editor says the Situation is, as a Situation with no identity.
The engine keeps the id of the Situation it holds, because the panel mints nothing.
A field standing empty says nothing was recorded, unless it still carries the mark it was loaded with.
Then it says instead that the user marked it as not known.

## `private void PScenarioStoreHandle(object sender, RoutedEventArgs e)`

Commits the held Situation, which stores it over the selected one or creates one nothing references yet.
Typing still waiting to be written is written first, so the commit carries the last keystroke.
Whether this is a create or a rewrite is the engine's reading of the draft, not the panel's.
Saving does not change the id, what references it, or the order it takes for any referrer.

## `private void PScenarioRemovalHandle(object sender, RoutedEventArgs e)`

Deletes the shown Situation.
A Situation nothing references is deleted outright.
One something references is deleted only after the user is told how many places that reaches and says yes.
The detaching and the delete are one operation in the store, because between two steps the count can change.
