# Rail

This folder holds the command rail every entry panel shares, so its buttons read the same in each.
The markup is unplaced, and the owning panel sets its grid cell and margin.
Its parts keep fixed names, so one driver finds them in whichever panel holds the rail.
The markup carries no hook, the C# shell only calls `InitializeComponent()`, and `QPanelRail` sets icons, commands, clicks and visibility.

## `PPanelRail`

### `PPanelRailFresh`

The new-record button stands first, because a new record comes before saving one.
Panels that never write a record here hide it through the driver.

### `PPanelRailVoyage`

The trail pair shows with the reader and the chronicle pair with the editor.
`PPanelRailChronicle` starts collapsed because the reader is the side shown first.

### `PPanelRailMarkup`

The import button stands just before export, so the two transfer actions sit together.
It starts collapsed because only the list panel imports, and that panel's driver `QLibrary` shows it and wires its click.

### `PPanelRailMode`

The two mode buttons carry no group name.
Radio buttons sharing one parent already exclude each other, and a fixed group name would join every rail on screen.

## Stamp

Commit: `c2f335c331fe`
- `PPanelRail.xaml` `38b87792cfecc596`
- `PPanelRail.xaml.cs` `dd3d79cc3f0f04e3`
