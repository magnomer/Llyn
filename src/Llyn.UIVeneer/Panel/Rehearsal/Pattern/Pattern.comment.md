# Pattern

The template editor.
Its markup carries no hook.

## `PStencil`

The template editor of the chosen deck, placed by `PDeck` in its own sheet and shelled as `PDeck` is.
It has one markup block for reading and editing, as `PCharter` has.
The grain line sits first, then the card sides, then the picture and sound boxes.
  Each side row holds its number, the custom mark, its field chips and the field box.
  A chip holds a caption, a join box and a remove button, and the field box carries the add label.
The bar holds the saved-template choice, the apply button and the save button.
The text toggle sits under the bar, and the slot rows below it stay for the text view.
The slot rows are a list in Conduct's order, and the driver sets each caption from the key Conduct chose.
  Each slot body sits in a bare input at the place and size of its text.
  The input stands beside the slot's caption and number.
  The caption column is one shared size group, so every body starts at one edge.

## Stamp

Commit: `2f9a06b8eb95`
- `PStencil.xaml` `9fc1ab73eaf74a58`
- `PStencil.xaml.cs` `055866f324882f6e`
