# QScenario.cs
Hash: `a32cda92d7792cce`

## `internal sealed class QScenario`

The edit area of the Repertoire panel, `PScenario`, with its three fields, its media rows and its tally chip.
Typing hands each field's text to its own gate on the playwright, which defers it on the tenure.
The gate answers the typed line, and the field's placeholder and twins are painted from it.
The engine answers with a bulletin, and the fields are redrawn from the draft.
A field already reading the same text is left alone by the text box itself, caret included.
The desk on the playwright holds the tenure from start to commit or cancel.
So the area keeps no draft id or timer.
It subscribes what it paints itself, so the owner `QRepertoire` only builds, introduces and lights it.

## `internal QScenario(UserControl scope)`

Takes the Repertoire page, and finds every `PScenario` part in it by contract ID.
It sets the two add buttons' icons and subscribes the three fields' typing.

## `internal void QScenarioDeskIntroduce(CRepertoire repertoire)`

`QRepertoireIntroduce` calls it once the repertoire Conduct exists.
It reads the playwright once off the repertoire, and reaches the scenario's gates and notices through it alone.
The picture and video drivers are handed the playwright's image and video gates, then their rows and add buttons.
A Situation has no card, so each add names a null card, and the draft routes it.
It subscribes the playwright's draft and scenario notices, which redraw the fields and the media rows.
The playwright shows a failed hold through the envoy.
It subscribes the atlas's rows event, which redraws the tally chip.

## `internal void QScenarioVisibleRefine()`

Shows the edit area while the diptych edits its parent.
The area is live only while `CRepertoireScenarioEnabled` holds.
The owner's mode refine calls it with the rest of the panel's mode.

## `private void QScenarioIntroduce()`

Subscribes the three fields' text changes.

## `private void QScenarioTeardown()`

Unsubscribes them while the fields are filled from a held Situation.
Filling a field raises the same change the user typing raises, and only typing may reach the gate.
So the area keeps no loading flag.

## `private void QScenarioTitleObserve(object sender, TextChangedEventArgs e)`

Hands the typed title to `CPlaywrightTitleSet` and paints the line it answers.

## `private void QScenarioKindObserve(object sender, TextChangedEventArgs e)`

Hands the typed kind to `CPlaywrightKindSet` and paints the line it answers.

## `private void QScenarioDescriptionObserve(object sender, TextChangedEventArgs e)`

Hands the typed description to `CPlaywrightDescriptionSet` and paints the line it answers.

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

Fills the three fields outright when a draft is opened or closed, and repaints the scenario's tally chip.
A closed draft arrives as the blank scenario, so the fields empty without a default here.

## `private void QScenarioFieldsRefine(CScenario scenario)`

Redraws every field and the media rows from the held Situation, with the fields unsubscribed.
The picture and video drivers redraw their own rows.
Each field's placeholder follows the key its line carries.

## `private static void QScenarioFieldRefine(TextBox field, CScenarioLine line)`

Writes one field's text and placeholder from its line.

## `private void QScenarioTallyRefine()`

Writes the chosen Situation's tally on the writing side of the panel.
`QVignette` writes the same figure on the reading side, so the reader and the writer see one figure.
The Conduct words the sentence through the engine, so a reference added elsewhere shows at the next row notice.
