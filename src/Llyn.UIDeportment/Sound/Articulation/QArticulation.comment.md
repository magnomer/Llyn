# QArticulation.cs
Hash: `bcbef32d8a85b281`

## `internal sealed partial class QArticulation`

Drives the IPA input aid.
It holds the two charts and the one thing they do, which is insert a character.
The charts themselves are built in the two files beside this one.
The aid itself is the veneer's `QArticulation` page, nested in the phonology page.

## `internal QArticulation(UserControl surface)`

Takes the page `QPhonology` pulled under the contract ID `QArticulation`.
It registers the page's own styles with `QLook`, so the glyph chip lights on hover and press.
It subscribes the lane's size change, then builds both charts.

## `private ScrollViewer QArticulationLane`

Each named part is pulled from the page by its contract ID on every read.

## `internal void QArticulationIntroduce(params TextBox[] fields)`

Names the pronunciation fields this aid may type into.
The aid follows the keyboard between them and types into the one last focused.
So it is tied to no single field instance and needs no rewiring when the editor reopens.

## Inline notes

### `private TextBox? _qArticulationField;`

The field a chosen character is inserted into.
It stands on the first registered field until the reader focuses another.
The search field is registered first, so a character typed before anything is focused lands in the search.

### `private TextBox? QArticulationTargetFind()`

The field last focused may have been hidden since, which is what the editor closing does to the pronunciation field.
A hidden field is skipped, and the first field still on screen takes the character instead.

### `private void QArticulationGlyphRefine(object sender, RoutedEventArgs e)`

A selection is replaced rather than left in place, which is what typing the character would do.
The caret is put after the inserted character, so a second character continues the transcription.

### `private void QArticulationLaneRefine(object sender, SizeChangedEventArgs e)`

The lane is asked for one row of charts.
It takes a second only when the two do not fit across it.
The consonant chart is the one that moves, because the vowel chart is the narrower of the two.
The widths come from what each card asked for rather than from what it was given.
The lane measures its content unbounded, so those widths stay the natural ones in either arrangement.
A chart not yet measured is left alone, so an early size change does not stack the charts by mistake.

### `private Grid QArticulationTableBuild(Grid table, int columns, int rows)`

One extra column and one extra row are added for the headers.
So a chart cell at column and row is placed one further along in both.

### `(Style)QArticulationRack.FindResource("Articulation.Header")`

The chart styles live in the page's own resources, which no ancestor of a new cell can see yet.
So they are looked up from the rack inside that markup.

### `private void QArticulationHeaderPlace(Grid table, string key, int column)`

The header text is a resource reference rather than a value.
The chart is built once, and the interface language may change after it is built.
