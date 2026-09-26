# PDisplayCompass.xaml

## `ResourceDictionary`

The floating contents column of the entry page: its row, its number and the row template.
The row answers a click the panel handles.
The Deportment class of the same name loads this markup and forwards the click to its host.
The host hands the click on to [LCompass](../../../Llyn.UIDeportment/Display/LCompass.comment.md).
The panel adds it from code after its own markup is parsed, so the list reads the template dynamically.

## `<Style x:Key="Theme.Compass.Choice" TargetType="Button">`

One heading of the page, lit under the pointer and coloured in the accent while its section is in view.
It is a button, because the row scrolls the page to the section it names.

## `<Style x:Key="Theme.Compass.Number" TargetType="TextBlock">`

The number before a heading that has one, at a fixed width so the names align.
It collapses when the heading carries no number.

## `<DataTemplate x:Key="Theme.Compass.Row">`

The row itself, indented by the depth of its heading, with the name trimmed to one line.
Its named parts are filled by the display, which also marks the current row `Chosen`.
