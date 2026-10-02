# PDisplayCompass.xaml
Hash: `aa1aeb296e1b4678`

## `ResourceDictionary`

The floating contents column of the entry page, with its row, its number and the row template.
The row answers a click the panel handles.
The display page merges this markup in its own resources.
The display hands the click on to [QCompass](../../../Llyn.UIDeportment/Display/QCompass.comment.md).

## `<Style x:Key="Theme.Compass.Choice" TargetType="Button">`

One heading of the page, lit under the pointer and coloured in the accent while its section is in view.
It is a button, because the row scrolls the page to the section it names.

## `<Style x:Key="Theme.Compass.Number" TargetType="TextBlock">`

The number before a heading that has one, at a fixed width so the names align.
It collapses when the heading carries no number.

## `<DataTemplate x:Key="Theme.Compass.Row">`

The row itself, indented by the depth of its heading, with the name trimmed to one line.
Its named parts are filled by the display, which also marks the current row `Chosen`.
