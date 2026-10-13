# Window

The window folder holds the application window layout, with navigation, content panels and the mention menu.
The markup carries no hook.
`PWindow` in Deportment pulls it by contract ID, subscribes every event and sets every icon.
Its `x:Class` shell lets `PBootstrap.xaml` offer it as the `PWindow` resource.
Its constructor only calls `InitializeComponent()`.

## `Window.Resources`

Merges the word menu dictionary, so the menu's list, row chrome and row template resolve in the window's scope.

## `PNavigation`

Groups workspace pages by entries, classification, sources, and the Xiesheng and Yunjing tools.
Each tab is a plain `Button` in the tab style, and none is marked chosen in markup.
`PNavigationDuplex` and `PNavigationSettings` keep dual-panel mode and settings below the divider, apart from workspace pages.

## `PEstablishment`

The establishment strip, placed as the veneer's own `PEstablishment` page.

## `PMentionMenu`

The shared popup used for word mentions and their suggestions.

## Stamp

Commit: `c2f335c331fe`
- `PWindow.xaml` `da36d6922532663a`
- `PWindow.xaml.cs` `d937759ae0d6b2e6`
