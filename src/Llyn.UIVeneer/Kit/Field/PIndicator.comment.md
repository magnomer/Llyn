# PIndicator.cs

## Inline notes

### `internal static class PIndicator`

Builds the scrollbar templates in code-owned XAML.
WPF requires the track to be named "PART_Track".
Keeping that framework contract here follows the same boundary as PField's "PART_ContentHost".
It prevents framework names from entering the audited XAML naming surface.

### `private static ControlTemplate PIndicatorTemplateBuild(Orientation orientation)`

One rail, drawn the same everywhere, laid along whichever axis it was asked for.
The rail is ten pixels of track with a thumb of the same ten pixels.
A thumb narrower than its track reads as a hairline lost inside an empty groove.
The four pixels of inset are the gap between the reading surface and the rail.
They belong to the rail rather than the page, so every scroll viewer keeps the same gap.

### `private static ControlTemplate PIndicatorGutterBuild()`

The scroll lane is fourteen pixels held beside the content, never over it.
Nothing the page draws is ever covered by the rail.
The lane is zero until the rail is actually shown, and only on the axis showing it.
An empty lane beside a short popup would read as a margin nobody asked for.
The computed visibility is what opens the lane, so a disabled or unneeded axis stays closed.
Each rail is given the lane size and the matching rail template here rather than by a style.
A style written beside the scroll viewer never reaches a rail the template makes.
The rail would keep its default thickness.
WPF requires the parts to be named "PART_ScrollContentPresenter", "PART_VerticalScrollBar" and "PART_HorizontalScrollBar".
They stay in code for the same reason the track name does.
