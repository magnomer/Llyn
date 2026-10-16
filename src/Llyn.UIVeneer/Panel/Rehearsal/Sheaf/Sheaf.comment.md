# Sheaf

The adder and the free-cue cell editor.
Its markup carries no hook.
The Deportment driver `QAccession` fills the adder `PAccession`.
The Deportment driver `QJotter` fills the cell editor `PJotter`.

## `PAccession`

The adder of the chosen deck, placed by `PDeck` and shelled as `PDeck` is.
The search field `PTrawl` and the add-all button `PAccessionBulk` sit in its top bar.
Each result row holds the headword, the epithet and one end slot.
  The end slot shows either the add button or the in-deck mark, in the same column.
The empty notice overlays the rows, and the tally line sits under them.

## `PJotter`

The cell editor of one free flashcard, placed by `PDeck` under the listing and shelled as `PDeck` is.
It has one markup block for reading and editing, as `PStencil` has.
  Each cell row holds the caption and number, the value in a bare input, and the pick button.
  The caption column is one shared size group, so every value starts at one edge.
  The input takes the tip `Cue.Cell`, and the button the text `Cue.Pick`.

## Stamp

Commit: `121691f60459`
- `PAccession.xaml` `6c5a5f15e59b48b4`
- `PAccession.xaml.cs` `4796bb51a37c4e27`
- `PJotter.xaml` `67395c4f79b45155`
- `PJotter.xaml.cs` `812cfcc29776a537`
