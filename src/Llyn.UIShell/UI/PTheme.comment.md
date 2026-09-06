# PTheme.xaml

## `<Style TargetType="ScrollBar">`

Scroll viewers use a clear, comfortably sized rail instead of the platform scrollbar.
Its leading margin leaves a gutter between the document and the rail.
It disappears with the scrollbar when no scrolling is needed.
The two control templates are registered by PIndicator.
WPF requires a PART_Track name.
That name must stay out of the audited XAML naming surface.

## `<Setter Property="Template" Value="{DynamicResource Theme.Input.Field.Template}" />`

The TextBox content host must be named "PART_ContentHost" (a WPF template contract).
That name is authored in code (PField) so it stays out of the audited XAML surface.

## `<Style x:Key="Theme.Input.Choice" TargetType="ComboBox">`

A field typed into that also offers what has been saved before it.
It reads as a plain field, because a value nothing ships is written far more often than it is picked.
The arrow is hidden while nothing has been saved, so an empty list never offers an empty menu.
Text search is off and the drop stays open while typing, so the list never overwrites what is being written.
WPF requires the editing box to be named "PART_EditableTextBox" and the drop "PART_Popup".
Both are template contracts and are held out of the audited XAML naming surface.

## `<Style x:Key="Theme.Text.Inverse" TargetType="TextBlock">`

Text inside an accent button.
The button's white Foreground only reaches content by inheritance.
The app-wide implicit TextBlock style (Theme.Ink) outranks that.
So the accent styles below restate it as an implicit style of their own.

## `<Style x:Key="Theme.Input.Second" TargetType="Button">`

The way out of an unsaved edit, and the shape both halves of the pair are cut from.
A surface inside a hairline is what this window already means by a button.
The label is a TextBlock the template owns, holding its colour as a local value.
The app-wide TextBlock style outranks anything inherited, so a presenter's own text would come out ink on accent.

## `<Style x:Key="Theme.Input.First" TargetType="Button">`

The one button that commits what has been written, and the only filled accent in its row.
It borrows the whole construction above and changes only what fills it.
Neither half dims to an opacity when it cannot be pressed.
Both drop their fill instead, keeping a faint edge and a faint label, so an idle header carries no weight.

## `<Style x:Key="Theme.Choice.Row" TargetType="Button">`

One row the user picks from.
It is a whole row that takes the click, used by the entry index and by the ordering dropdown.
Keeping it templated avoids falling back to the platform's square, grey Button chrome.
This is not a picker-popup row.
Those carry no click of their own and offer their actions separately.
They are Theme.Popup.RowSurface.

## `<Setter Property="Background" Value="{StaticResource Theme.Surface}" />`

The rows sit on the panel's own ground, so they carry a card of their own.
The hover trigger below still repaints it.

## `<Style x:Key="Theme.Popup.Surface" TargetType="Border">`

The floating ground a picker popup stands on.
It is the same white as a field, so the rows inside need no card of their own.
The list reads as one surface instead of a stack of tiles.

## `<Style x:Key="Theme.Popup.Title" TargetType="TextBlock">`

What the popup is, said once and quietly: the rows below are the content, not this line.

## `<Style x:Key="Theme.Popup.Notice" TargetType="TextBlock">`

The one line the popup says while it has no rows to show: searching, or nothing found.

## `<Style x:Key="Theme.Popup.Progress" TargetType="Border">`

The search running, as a hairline under the title rather than a box of its own.

## `<Style x:Key="Theme.Popup.RowSurface" TargetType="Border">`

One row of a picker popup.
It carries no border and no fill of its own.
A row can offer more than one action, so every action is its own button.
The row itself takes no click.
Transcriber and downloader rows are the same row.

## `<Style x:Key="Theme.Popup.RowAction" TargetType="Button">`

The action that takes a row's offer.
One of these sits on every row, so it is quiet until it is pointed at.
It is text alone, tinted only on hover.

## `<Style.Resources>`

The app-wide implicit TextBlock style outranks the button's inherited Foreground.
So the label is bound back to the button and follows its enabled and hover states.

## `<Style x:Key="Theme.Popup.IconAction" TargetType="Button">`

A row's second action, carrying an icon instead of a word.
Same quiet-until-pointed-at treatment as the taking button beside it.

## `<Style x:Key="Theme.Language.Chip" TargetType="Border">`

The blue pill an entry's language is named on.
The reading view wears it and the editor's language toggle copies it.
Both are defined here, so the two views cannot drift apart.

## `<Style x:Key="Theme.Language.Toggle" TargetType="ToggleButton">`

The editor's language pill, drawn as the reading view's pill is drawn.
It stays a toggle, because a language is chosen here and only reported there.
The border is transparent until pointed at or opened, which is the only hint of the menu it holds.
