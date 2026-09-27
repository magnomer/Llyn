# QTally.cs

## `internal sealed class QTally`

One printed line of a section's tally on the Diwei page: language, kind and the marks of the chosen set.
It is copied from the Conduct tally, which already holds only the set the section shows.

## `public IReadOnlyList<QTallyMark> QTallyMarks { get; }`

The marks of the chosen set, each drawn as the part text with its count raised after it.
Each carries its characters, listed in the popup the part opens.

## `internal static void QTallyApply(FrameworkElement container, object item, string? _)`

Fills a tally line: language, kind and the marks, whose list is attached to `QTallyMark.QTallyMarkApply`.

## `internal static IReadOnlyList<QTally> QTallyBuild(IReadOnlyList<CTally> tallies)`

Wraps each tally of one section, in the section's order.
