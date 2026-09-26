# PWindow.xaml

The application window layout, including navigation, content panels, and mention menu.
The markup carries no hook.
`PWindow` in Deportment loads it, subscribes every event and sets every icon.

## `PRoof`

Holds the product mark, about and exit menu, and window caption buttons.

## `PHouse`

Places the navigation rail beside the active workspace panel.

## `PNavigation`

Groups workspace pages by entries, classification, sources, and reference tools.

## `PNavigationDuplex` and `PNavigationSettings`

Keep dual-panel mode and settings below the divider, apart from workspace pages.

## `PPanel`

Hosts the workspace pages selected from the navigation rail.

## `PMentionMenu`

Provides the shared popup used for word mentions and their suggestions.
