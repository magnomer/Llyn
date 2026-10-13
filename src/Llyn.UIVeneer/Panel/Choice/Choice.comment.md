# Choice

This folder holds the language filter and the ordering picker that every browse panel shares.
They are shared so their buttons and menus read the same in each panel.
The markup is unplaced, and the owning panel sets its grid cell.
Parts keep fixed names, so one driver finds them in whichever panel holds the control.
The markup carries no hook, and each C# shell only calls `InitializeComponent()`.

## `PChoiceFilter`

`QChoiceFilter` sets the icon, the tooltip, the rows and the mark.
The button carries no tooltip, because the driver sets it from the title the owner names.
`PChoiceFilterMark` starts collapsed, because nothing is filtered before a vista is read.
The popup hangs from the button and is shifted left, so it opens under the bar's right end.
The menu starts empty, because the driver builds it from the languages a load answers.

## `PChoiceOrder`

`QChoiceOrder` sets the icon, the tooltip, the rows and the popup's anchor.
The button carries no tooltip, because the driver sets it from the title the owner names.
`PChoiceOrderList` starts empty, because the driver builds one row per ordering the owner offers.

## Stamp

Commit: `c2f335c331fe`
- `PChoiceFilter.xaml` `b9d3b7d662c9ee23`
- `PChoiceFilter.xaml.cs` `9fcc982d901d80c0`
- `PChoiceOrder.xaml` `3cc3fc84935eb03b`
- `PChoiceOrder.xaml.cs` `5cf628dbffc0f1b1`
