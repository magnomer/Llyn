# PChoiceFilter.xaml
Hash: `b9d3b7d662c9ee23`

## `<Grid>`

The language filter every browse panel shares, so its button and menu read the same in each.
The markup is unplaced, and the owning panel sets its grid cell.
Its parts keep fixed names, so one driver finds them in whichever panel holds the filter.

## `<ToggleButton x:Name="PChoiceFilterDropper" ...>`

The button carries no tooltip, because the driver sets it from the title the owner names.
`PChoiceFilterMark` starts collapsed, because nothing is filtered before a vista is read.

## `<Popup x:Name="PChoiceFilterDropdown" ...>`

The popup hangs from the button and is shifted left, so it opens under the bar's right end.
The menu starts empty, because the driver builds it from the languages a load answers.

## Hooks

The markup carries no hook.
The Deportment driver `QChoiceFilter` sets the icon, the tooltip, the rows and the mark.
