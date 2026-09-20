# LTallyLine.cs

## `public sealed record LTallyLine(`

One line of a tally, a language and a kind: the parts its readings take, in IPA and in respelling.
A language sorting its readings by kind, as Japanese by Go-on and Kan-on, gets one line per kind.
Both sets are always carried, so the view's switch only picks which is printed.

**Parameters**

- `LTallyLineLanguage` — The borrowing language, such as `Korean` or `Xiang`.
- `LTallyLineKind` — The kind of reading the line counts, such as `Go-on`, or empty for a language without kinds.
- `LTallyLineIpa` — The marks cut from the readings as fetched, most characters first, then by part text.
- `LTallyLineRespelling` — The marks cut from the respellings, in the same order.

## `public IReadOnlyList<LTallyMark> LTallyLineRead(bool respelled)`

The marks of one set: the respelling marks when asked for, else the IPA marks.

## `public static IReadOnlyList<LTallyLine> LTallyLineScan(`

The lines of one division: the reflex parts of every character gathered per language and kind, both sets apart.
`kind` picks the part, onset for an initial and vowel with coda for a rime, off each row's stored anatomy.
A character absent from `readings` adds nothing, and an empty part or blank language records nothing.
Languages sort by their place in `ranking`, unranked ones after by name, then kinds as the rows first name them.
A line whose two sets are both empty is left out, and each line lists its marks by count.

## `private static void LTallyLineAdd(`

Records one character under one language, kind and part, once however many readings take it.
