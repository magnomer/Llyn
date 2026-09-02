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

The pronunciation field a chosen character is inserted into.
It stands on the first registered field until the reader focuses another.

### `private void PArticulationInsert(string character)`

A selection is replaced rather than left in place, which is what typing the character would do.
The caret is put after the inserted character, so a second character continues the transcription.

### `private Grid PArticulationTableBuild(Grid table, int columns, int rows)`

One extra column and one extra row are added for the headers.
So a chart cell at column and row is placed one further along in both.

### `private void PArticulationHeaderPlace(Grid table, string key, int column)`

The header text is a resource reference rather than a value.
The chart is built once, and the interface language may change after it is built.
