# PThemeReflexEditor.xaml

The editor's side of a reflex row: the bare fields, the hover controls, the rebuild button and the row template.
It merges the shared row styles, so a field stands where the reading view prints its text.
A cell grows its box only while edited, its tag naming the box style.
So no measure style is declared here.

## `<Style x:Key="Theme.Reflex.Anchoring" TargetType="Button">`

The editor's hand on the anchors: the same anchor text made clickable, opening the dropdown through the command.
While the row is anchored to nothing it prints the muted prompt instead, so there is something to click.
It hovers in the accent colour and is collapsed on a row that may not be anchored.

## `<Style x:Key="Theme.Reflex.Field" TargetType="TextBox">`

The bare field editing a row's reading, standing where the reading view prints it, accented when marked.

## `<Style x:Key="Theme.Reflex.Prompt" TargetType="TextBlock">`

The text block of the reading cell, showing the muted placeholder while the row's text is blank.
So an empty cell still measures, and the box that grows over it has a width to take.

## `<Style x:Key="Theme.Reflex.Name" TargetType="TextBox">`

The bare field editing a row's language, with the row's region as its tooltip when there is one.
It takes the label size and colour, so it reads as the label does.
It overhangs its unseen twin by the two pixels a field pads its text with.
So the column is as wide as the view's.

## `<Style x:Key="Theme.Reflex.Aside" TargetType="TextBox">`

The bare field editing a row's kind, romanization, meaning or note, in its matching size and colour.
It overhangs its unseen twin as the language field does, for the same reason.

## `<Style x:Key="Theme.Reflex.Star" TargetType="Button">`

The hover control marking a row as the reading in common use, lit in the accent when it is.

## `<Style x:Key="Theme.Reflex.Addition" TargetType="Button">`

The hover plus adding a row after this one in the same language.

## `<Style x:Key="Theme.Reflex.Remove" TargetType="Button">`

The hover minus dropping this row.

## `<Style x:Key="Theme.Reflex.Rebuild" TargetType="Button">`

The button at the top right corner of the editor's reflex block that drops the rows and fetches them again.
It is drawn as the icon commands of a meaning card are: ink at rest, lit only under the pointer.
While the button's tag is true the icon turns in place once a second, and stops when the tag falls.
The editor sets the tag from the engine's fill state, so the icon turns as long as the fetch runs.

## `<DataTemplate x:Key="Theme.Reflex.Control">`

The hover star, plus and minus of a row, stacked as the accent controls of a pronunciation row are.
The row's slot names it by tag, and the handles are built into the slot on first hover or focus.

## `<DataTemplate x:Key="Theme.Reflex.Row">`

One editor row with the language, kind, reading, romanization, meaning and note fields.
The anchor button, hover star, plus and minus follow them.
Every field is bare and stands where the reading view prints its text.
The side cells are the same grids in both templates.
So an empty cell measures alike in both modes.
The region is no field, only the hover the view has on the language.
The reading field of a phonemic language stands between two slashes it does not hold.
