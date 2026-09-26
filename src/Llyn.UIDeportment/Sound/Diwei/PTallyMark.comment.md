# PTallyMark.cs

## `internal sealed class PTallyMark`

One tally mark of a Diwei section as the surface draws it: its text, its count and its characters.
It stands between the engine's `LTallyMark` and the surface, so no engine type reaches a fill.

## `private PTallyMark(LTallyMark mark)`

Copies the mark and writes its count as invariant text once.

## `public string PTallyMarkText { get; }`

The mark itself, such as an initial or a rime.

## `public string PTallyMarkCount { get; }`

How many characters bear the mark, as the small number after it.

## `public IReadOnlyList<string> PTallyMarkCharacters { get; }`

The characters that bear the mark, listed in its dropdown.

## `internal static void PTallyMarkApply(FrameworkElement container, object item, string? _)`

Fills a mark chip: the toggle's text and count, and the dropdown's characters.
The toggle opens the dropdown, and a dropdown closed from outside clears the toggle.

## `private static void PTallyDropHandle(object sender, RoutedEventArgs e)`

Opens or closes the dropdown beside the toggle as the toggle is checked or cleared.

## `private static void PTallyCloseHandle(object? sender, EventArgs e)`

Clears the toggle when its dropdown closes on its own, such as on a click outside.

## `internal static IReadOnlyList<PTallyMark> PTallyMarkBuild(IReadOnlyList<LTallyMark> marks)`

Wraps each engine mark of a tally line, in the engine's order.
