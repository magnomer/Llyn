# PPanelRail.xaml
Hash: `38b87792cfecc596`

## `<local:QRail>`

The command rail every entry panel shares, so its buttons read the same in each.
The markup is unplaced, and the owning panel sets its grid cell and margin.
Its parts keep fixed names, so one driver finds them in whichever panel holds the rail.

## `<Button x:Name="PPanelRailFresh" ...>`

The new-record button stands first, because a new record comes before saving one.
Panels that never write a record here hide it through the driver.

## `<StackPanel x:Name="PPanelRailVoyage" ...>`

The trail pair shows with the reader and the chronicle pair with the editor.
`PPanelRailChronicle` starts collapsed because the reader is the side shown first.

## `<Button x:Name="PPanelRailMarkup" ...>`

The import button stands just before export, so the two transfer actions sit together.
It starts collapsed because only the list panel imports.
That panel's driver `QLibrary` shows it and wires its click.

## `<Border x:Name="PPanelRailMode" ...>`

The two mode buttons carry no group name.
Radio buttons sharing one parent already exclude each other, and a fixed group name would join every rail on screen.

## Hooks

The markup carries no hook.
The Deportment driver `QPanelRail` sets icons, commands, clicks and visibility.
