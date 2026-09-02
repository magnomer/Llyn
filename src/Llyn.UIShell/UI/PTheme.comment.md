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

## `<Style x:Key="Theme.Text.Inverse" TargetType="TextBlock">`

Text inside an accent button.
The button's white Foreground only reaches content by inheritance.
The app-wide implicit TextBlock style (Theme.Ink) outranks that.
So the accent styles below restate it as an implicit style of their own.

## `<Style x:Key="Theme.Choice.Row" TargetType="Button">`

One row the user picks from.
It is a whole row that takes the click, used by the entry index and by the ordering menu.
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
Lookup and downloader rows are the same row.

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
