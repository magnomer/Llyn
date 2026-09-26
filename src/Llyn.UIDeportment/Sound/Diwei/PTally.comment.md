# PTally.cs

## `internal sealed class PTally`

One printed line of a section's tally on the Diwei page: language, kind and the marks of the chosen set.
It is copied from the engine's tally line, asking that line for the set the section shows.

## `public IReadOnlyList<PTallyMark> PTallyMarks { get; }`

The marks of the chosen set, each drawn as the part text with its count raised after it.
Each carries its characters, listed in the popup the part opens.

## `internal static void PTallyApply(FrameworkElement container, object item, string? _)`

Fills a tally line: language, kind and the marks, whose list is attached to `PTallyMark.PTallyMarkApply`.

## `internal static IReadOnlyList<PTally> PTallyBuild(IReadOnlyList<LTallyLine> lines, bool respelled)`

A plain copy loop over the tally lines of one section, the respelling set when `respelled`.
