# QRepertoireEditor.cs

## `internal sealed partial class QRepertoire`

The edit area of the Repertoire panel, `QScenario`, and what its three fields do.
Typing hands the three texts to the desk's text gate through `QScenarioChangeDefer`.
The gate builds one whole-form body and keeps a still unknown field unknown.
The engine answers with a bulletin, and the fields are redrawn from the draft.
A field already reading the same text is left alone by the text box itself, caret included.

## `private void QScenarioAttach()`

Subscribes the three fields' text changes.

## `private void QScenarioDetach()`

Unsubscribes them while the fields are filled from a held Situation.
Filling a field raises the same change the user typing raises, and only typing may reach the gate.
So the panel keeps no loading flag.

## `private static string QScenarioHintRead(string key, bool unknown)`

The placeholder one field shows while it is empty.
An unknown field shows the unknown mark, and a never-written one asks for what it wants.
The title asks with the untitled text the catalog uses, so an empty title reads the same in both places.

## `private void QScenarioHintApply()`

Feeds the two unseen twins of the title, the work their bindings did before.
The hint copies the placeholder and shows only while the title is empty, so the head row closes on it.
The deportment answers whether the title is empty, so the view measures no input.
The ghost copies the written title, so the head row closes on the text.
It runs after the title or its placeholder changes.

## `private void QScenarioMeasureApply()`

Feeds the unseen twin of the kind with the written kind, or with the placeholder while the kind is empty.
The deportment picks between the two, and the chip closes on that text.

## `private void QScenarioApply(CSituationDraft? situation)`

Fills the three fields outright from a Situation, or empties them, when a draft is opened or closed.
The picture and video rows are filled with them, so an opened draft shows every row it holds.

## `private void QScenarioShow(CSituationDraft situation)`

Redraws every field and the media rows from the held Situation, with the fields unsubscribed.
Each field's placeholder follows whether its value is unknown.

## `private static void QScenarioFieldShow(TextBox field, string key, CStateValue value)`

Writes one field's text and placeholder from its held value.
