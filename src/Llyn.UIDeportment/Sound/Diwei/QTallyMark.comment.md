# QTallyMark.cs

## `internal sealed class QTallyMark`

One tally mark of a Diwei section as the surface draws it: its text, its count and its characters.
It is copied from the Conduct mark, so no engine type reaches a fill.

## `private QTallyMark(CTallyMark mark)`

Copies the mark and writes its count as invariant text once.

## `public string QTallyMarkText { get; }`

The mark itself, such as an initial or a rime.

## `public string QTallyMarkCount { get; }`

How many characters bear the mark, as the small number after it.

## `public IReadOnlyList<string> QTallyMarkCharacters { get; }`

The characters that bear the mark, listed in its dropdown.

## `internal static void QTallyMarkRefine(FrameworkElement container, object item, string? _)`

Fills a mark chip: the toggle's text and count, and the dropdown's characters.
The toggle opens the dropdown, and a dropdown closed from outside clears the toggle.

## `private static void QTallyDropRefine(object sender, RoutedEventArgs e)`

Opens or closes the dropdown beside the toggle as the toggle is checked or cleared.

## `private static void QTallyCloseRefine(object? sender, EventArgs e)`

Clears the toggle when its dropdown closes on its own, such as on a click outside.

## `internal static IReadOnlyList<QTallyMark> QTallyMarkBuild(IReadOnlyList<CTallyMark> marks)`

Wraps each mark of a tally, in the tally's order.
