# PDisplayCommand.cs

## `public static class PDisplayCommand`

The routed commands raised over the shared display, bound by every panel that shows an entry.
A command rather than a click, so each panel enables the button by the same check it uses for print.

## `public static RoutedCommand PDisplayCommandPortrait { get; }`

Exports a portrait of the entry the panel is reading.
