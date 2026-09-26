# PThemeReflexEditor.xaml

The editor's side of a reflex row: the bare fields, the hover controls, the rebuild button and the row template.
It merges the shared row styles, so a field stands where the reading view prints its text.
A cell grows its box only while edited, and the row fill tags it with the box style.
So no measure style is declared here.

## `<Style x:Key="Theme.Reflex.Anchoring" TargetType="Button">`

The editor's hand on the anchors: the same anchor text made clickable, opening the dropdown through the command.
The row fill sets the anchor text as its content and collapses it where anchoring is barred.
`PLook` rows copy the content to the text, show the muted prompt while it is empty, and light the hover.
Its command and parameter are `PLook` rows too.

## `<Style x:Key="Theme.Reflex.Field" TargetType="TextBox">`

The bare field editing a row's reading, standing where the reading view prints it.
The reading cell shows the muted placeholder while blank, so an empty cell still measures.

## `<Style x:Key="Theme.Reflex.Lead" TargetType="TextBox">`

The reading field of a row marked as the reading in common use, in the accent colour.
The row fill names it in the cell's tag in place of the plain field.

## `<Style x:Key="Theme.Reflex.Name" TargetType="TextBox">`

The bare field editing a row's language, taking over the tooltip of the label it covers.
It takes the label size and colour, so it reads as the label does.
It overhangs its unseen twin by the two pixels a field pads its text with.
So the column is as wide as the view's.

## `<Style x:Key="Theme.Reflex.Aside" TargetType="TextBox">`

The bare field editing a row's kind, romanization, meaning or note, in its matching size and colour.
It overhangs its unseen twin as the language field does, for the same reason.

## `<Style x:Key="Theme.Reflex.Star" TargetType="Button">`

The hover control marking a row as the reading in common use.
Its command and star icon are `PLook` rows.

## `<Style x:Key="Theme.Reflex.Addition" TargetType="Button">`

The hover plus adding a row after this one in the same language, its command a `PLook` row.

## `<Style x:Key="Theme.Reflex.Remove" TargetType="Button">`

The hover minus dropping this row, with the reflex command as a `PLook` row.

## `<Style x:Key="Theme.Reflex.Rebuild" TargetType="Button">`

The button at the top right corner of the editor's reflex block that drops the rows and fetches them again.
It is the shared regenerate button, placed here, with the renewal command as a `PLook` row.
The editor sets the tag from the engine's fill state, so the mark turns as long as the fetch runs.

## `<DataTemplate x:Key="Theme.Reflex.Control">`

The hover star, plus and minus of a row, stacked as the accent controls of a pronunciation row are.
The row fill names it in the slot's tag.
The handles are built into the slot on first hover or focus.

## `<DataTemplate x:Key="Theme.Reflex.Row">`

One editor row with the language, kind, reading, romanization, meaning and note fields.
The anchor button, hover star, plus and minus follow them.
Every field is bare and stands where the reading view prints its text.
The side cells are the same grids in both templates.
So an empty cell measures alike in both modes.
The region is no field, only the hover the view has on the language.
The reading field of a phonemic language stands between two slashes it does not hold.
`LReflexItem.LReflexItemApply` fills every named part and picks the reading field's style.
