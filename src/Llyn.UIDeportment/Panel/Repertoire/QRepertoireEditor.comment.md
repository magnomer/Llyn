# QRepertoireEditor.cs
Hash: `7034d050e8a5a2df`

## `internal sealed partial class QRepertoire`

The edit area of the Repertoire panel, `QScenario`, and what its three fields do.
Typing hands each field's text to its own gate on the repertoire, which defers it on the tenure.
The gate answers the typed line, and the field's placeholder and twins are painted from it.
The engine answers with a bulletin, and the fields are redrawn from the draft.
A field already reading the same text is left alone by the text box itself, caret included.

## `private void QScenarioIntroduce()`

Subscribes the three fields' text changes.

## `private void QScenarioDetach()`

Unsubscribes them while the fields are filled from a held Situation.
Filling a field raises the same change the user typing raises, and only typing may reach the gate.
So the panel keeps no loading flag.

## `private void QScenarioTitleObserve(object sender, TextChangedEventArgs e)`

Hands the typed title to `CRepertoireTitleSet` and paints the line it answers.

## `private void QScenarioKindObserve(object sender, TextChangedEventArgs e)`

Hands the typed kind to `CRepertoireKindSet` and paints the line it answers.

## `private void QScenarioDescriptionObserve(object sender, TextChangedEventArgs e)`

Hands the typed description to `CRepertoireDescriptionSet` and paints the line it answers.

## `private void QScenarioTitleRefine(CScenarioLine line)`

Paints the title's placeholder and its two unseen twins from a typed line.

## `private void QScenarioKindRefine(CScenarioLine line)`

Paints the kind's placeholder and its measuring twin from a typed line.

## `private void QScenarioDescriptionRefine(CScenarioLine line)`

Paints the description's placeholder from a typed line.

## `private void QScenarioHintRefine(CScenarioLine title)`

Feeds the two unseen twins of the title, the work their bindings did before.
The hint copies the placeholder and shows only while the line is vacant, so the head row closes on it.
The ghost copies the written title, so the head row closes on the text.

## `private void QScenarioMeasureRefine(CScenarioLine kind)`

Feeds the unseen twin of the kind with the written kind.
While the kind is empty, it takes the line's wording instead.

## `private void QScenarioRefine(CScenario scenario)`

Fills the three fields outright when a draft is opened or closed, and repaints the tally chips.
A closed draft arrives as the blank scenario, so the fields empty without a default here.

## `private void QScenarioFieldsRefine(CScenario scenario)`

Redraws every field and the media rows from the held Situation, with the fields unsubscribed.
Each field's placeholder follows the key its line carries.

## `private static void QScenarioFieldRefine(TextBox field, CScenarioLine line)`

Writes one field's text and placeholder from its line.
