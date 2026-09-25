# PThemePopup.xaml

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

## `<Style x:Key="Theme.Popup.ProgressBar" TargetType="Border">`

The sliding bar inside that hairline.
The phonetician and the downloader run the same search, so they share one bar rather than declaring two.

## `<Style x:Key="Theme.Popup.RowSurface" TargetType="Border">`

One row of a picker popup.
It carries no border and no fill of its own.
A row can offer more than one action, so every action is its own button.
The row itself takes no click.
Phonetician and downloader rows are the same row.

## `<Style x:Key="Theme.Popup.RowReading" TargetType="Button">`

One reading on a phonetician row, taken by clicking it.
It presents whatever content the reading gives it, a flag or a name beside the transcription.
The taking button carries one word and cannot hold an image, which is why this is a second style.
It lights up the same way as every other row action when pointed at.

## `<Style x:Key="Theme.Popup.IconAction" TargetType="Button">`

A row's second action, carrying an icon instead of a word.
Same quiet-until-pointed-at treatment as the taking button beside it.
