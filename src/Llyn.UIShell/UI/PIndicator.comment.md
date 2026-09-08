# PIndicator.cs

## Inline notes

### `internal static class PIndicator`

Builds the scrollbar templates in code-owned XAML.
WPF requires the track to be named "PART_Track".
Keeping that framework contract here follows the same boundary as PField's "PART_ContentHost".
It prevents framework names from entering the audited XAML naming surface.

### `private static ControlTemplate PIndicatorGutterBuild(string size = PIndicatorGutterSize)`

The scroll gutter is a fixed lane the rail lives in.
It is held open whether or not the rail is shown.
A document keeps the same width no matter how much of it there is.
Nothing the page draws is ever covered by the rail.
The lane collapses only on the axis the scroll viewer disables.
The default lane is 40 pixels; the catalog template reserves 6 pixels for a locally compact scrollbar.
The catalog rail and thumb use that full width without internal margins.
WPF requires the parts to be named "PART_ScrollContentPresenter", "PART_VerticalScrollBar" and "PART_HorizontalScrollBar".
They stay in code for the same reason the track name does.
