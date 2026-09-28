# QDiwei.cs

## `internal sealed class QDiwei`

The driver of the category page, over the Veneer's `PDiwei` surface the yunjing panel places.
Its list is attached to `QDiweiItem.QDiweiItemApply`, which fills each section and the lists inside it.

## `private CAtelier _qDiweiAtelier = null!;`

The atelier, asked only for the language fonts.

## `private CYunjing _cYunjing = null!;`

The yunjing session, whose gates answer a chip and the switch.

## `internal QDiwei(UserControl surface)`

Keeps the surface and binds the entry and switch commands the chips raise, then attaches the section fill.

## `private TextBlock QDiweiHeadword`

Each named part of the surface is read through `QContract.QContractFind`.

## `internal void QDiweiAttach(CAtelier atelier, CYunjing yunjing)`

Keeps the atelier for the fonts and the yunjing session for the gates.

## `internal void QDiweiShow(CDiweiPage page, string kind)`

Draws the headword line for the page and copies its sections into rows.
The headword takes the language's headword font and the list its glyph font.
The kind chip reads the resource under the key the session names.
An empty page shows the empty notice instead.

## `private void QDiweiSwitchHandle(object sender, ExecutedRoutedEventArgs e)`

Hands a click on either half of any section's switch to the tally gate, the parameter saying which half.

## `private void QDiweiEntryHandle(object sender, ExecutedRoutedEventArgs e)`

Hands a character chip to the glyph gate.
