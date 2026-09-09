# PArticulation.xaml.cs

## `public partial class PArticulation : UserControl`

The IPA input aid as a control.
It holds the two charts and the one thing they do, which is insert a character.
The charts themselves are built in the two files beside this one.

## `internal void PArticulationAttach(params TextBox[] fields)`

Names the pronunciation fields this aid may type into.
The aid follows the keyboard between them and types into the one last focused.
So it is tied to no single field instance and needs no rewiring when the editor reopens.

## Inline notes

### `private TextBox? _pArticulationField;`

The field a chosen character is inserted into.
It stands on the first registered field until the reader focuses another.
The search field is registered first, so a character typed before anything is focused lands in the search.

### `private TextBox? PArticulationTargetFind()`

The field last focused may have been hidden since, which is what the editor closing does to the pronunciation field.
A hidden field is skipped, and the first field still on screen takes the character instead.

### `private void PArticulationInsert(string character)`

A selection is replaced rather than left in place, which is what typing the character would do.
The caret is put after the inserted character, so a second character continues the transcription.

### `private void PArticulationPlace(double lane)`

The lane is asked for one row of charts and takes a second only when the two do not fit across it.
The consonant chart is the one that moves, because the vowel chart is the narrower of the two.
The widths come from what each card asked for rather than from what it was given.
The lane measures its content unbounded, so those widths stay the natural ones in either arrangement.
A chart not yet measured is left alone, so an early size change does not stack the charts by mistake.

### `private Grid PArticulationTableBuild(Grid table, int columns, int rows)`

One extra column and one extra row are added for the headers.
So a chart cell at column and row is placed one further along in both.

### `private void PArticulationHeaderPlace(Grid table, string key, int column)`

The header text is a resource reference rather than a value.
The chart is built once, and the interface language may change after it is built.
