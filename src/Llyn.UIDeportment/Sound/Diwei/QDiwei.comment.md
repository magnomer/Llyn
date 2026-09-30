# QDiwei.cs

## `internal sealed class QDiwei`

The driver of the category page, over the Veneer's `PDiwei` surface the yunjing panel places.
Its list is attached to `QDiweiItem.QDiweiItemRefine`, which fills each section and the lists inside it.

## `private CYunjing _cYunjing = null!;`

The yunjing session, whose gates answer a chip and the switch.

## `internal QDiwei(UserControl surface)`

Keeps the surface and binds the entry and switch commands the chips raise, then attaches the section fill.

## `private TextBlock QDiweiHeadword`

Each named part of the surface is read through `QContract.QContractFind`.

## `internal void QDiweiIntroduce(CYunjing yunjing)`

Keeps the yunjing session for the gates.

## `internal void QDiweiRefine(CDiweiPage page, string kind)`

Draws the headword line for the page and copies its sections into rows.
The headword takes the page's ready headword font and the list its ready glyph font.
The kind chip reads the resource under the key the session names.
An empty page shows the empty notice instead.

## `private void QDiweiSwitchObserve(object sender, ExecutedRoutedEventArgs e)`

Hands a click on either half of any section's switch to the tally gate, the parameter saying which half.

## `private void QDiweiEntryObserve(object sender, ExecutedRoutedEventArgs e)`

Hands a character chip to the glyph gate.
