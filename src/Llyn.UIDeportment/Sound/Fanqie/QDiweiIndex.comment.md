# QDiweiIndex.cs
Hash: `97b9151e81a415bd`

## `internal sealed class QDiweiIndex`

The onset and rime columns of the yunjing panel, each with its search field over it.
Picking a cell in either column opens its category page.
It subscribes what it paints itself, so the owner `QYunjing` only builds and introduces it.
The ordering pickers over both columns stay with the owner, as shared `QChoiceOrder` parts.

## `internal QDiweiIndex(UserControl surface)`

Takes the yunjing page, and finds both columns, their empty lines and both search fields by contract ID.
It sets the search hints, attaches the row fills and subscribes the search fields and the row clicks.

## `private TextBox QPlumb`

Each named part is pulled from the page by its contract ID on every read.

## `internal void QDiweiIndexIntroduce(CYunjing yunjing, CAtelier atelier)`

`QYunjingIntroduce` calls it once the Conduct yunjing exists.
It subscribes the area's cell opening and change and the workspace opening, then binds both lists.

## `private void QShengmuRefine()`

Lists the initial column afresh with the empty text the session names.
It answers the area's change and the workspace's opening.

## `private void QYunmuRefine()`

Lists the rime column afresh with the empty text the session names.
It answers the area's change and the workspace's opening.

## `private void QPlumbRefine()`

Answers the area's opening of a cell a fanqie chip names by emptying the onset search field.
The area has already emptied the query, so the field only shows it.
The field's own handler still hears the change, and its gate finds the query already empty.

## `private void QFathomRefine()`

Empties the rime search field on the same opening, for the same reason.

## `private void QPlumbObserve(object sender, TextChangedEventArgs e)`

A change of the initial search field hands its text to the find gate.

## `private void QFathomObserve(object sender, TextChangedEventArgs e)`

A change of the rime search field hands its text to the find gate.

## `private void QDiweiIndexObserve(object sender, RoutedEventArgs e)`

A click on either column hands the cell's id and final flag to the select gate.
