# PTally.cs

## `internal sealed class PTally`

One printed line of a division's tally on the Diwei page: language, kind and the marks of the chosen set.
It is built from the engine's [LTally](../../Llyn.Core/Pronunciation/LTally.comment.md) for the set the switch shows.

## `public string PTallyLanguage { get; }`

The borrowing language the line stands for.

## `public string PTallyKind { get; }`

The kind of reading, such as `Go-on`, printed after the language, or empty.

## `public IReadOnlyList<LTallyMark> PTallyMarks { get; }`

The marks of the chosen set, each drawn as the part text with its count raised after it.
Each carries its characters, listed in the popup the part opens.

## `internal static IReadOnlyList<PTally> PTallyScan(LTally? tally, bool respelled)`

The lines of one division, the respelling set when `respelled`, else the IPA set.
A line whose chosen set is empty prints nothing, and a missing tally prints none.
