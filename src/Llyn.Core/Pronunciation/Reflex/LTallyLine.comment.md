# LTallyLine.cs
Hash: `4beb0ccc1b3239f5`

## `public sealed record LTallyLine(string LTallyLineLanguage, string LTallyLineKind, IReadOnlyList<LTallyMark> LTallyLineIpa, IReadOnlyList<LTallyMark> LTallyLineRespelling)`

One line of a tally, a language and a kind.
It holds the parts its readings take, in IPA and in respelling.
A language sorting its readings by kind, as Japanese by Go-on and Kan-on, gets one line per kind.
Both sets are always carried, so the view's switch only picks which is printed.

**Parameters**

- `LTallyLineLanguage` — The borrowing language, such as `Korean` or `Xiang`.
- `LTallyLineKind` — The kind of reading the line counts, such as `Go-on`, or empty for a language without kinds.
- `LTallyLineIpa` — The marks cut from the readings as fetched, most characters first, then by part text.
- `LTallyLineRespelling` — The marks cut from the respellings, in the same order.

## `public IReadOnlyList<LTallyMark> LTallyLineRead(bool respelled)`

The marks of one set, the respelling marks when asked for, else the IPA marks.

## `public static IReadOnlyList<LTallyLine> LTallyLineScan(string kind, IReadOnlyList<string> characters, IReadOnlyDictionary<string, IReadOnlyList<LReflex>> readings, LReflexOrder order)`

The lines of one division.
The reflex parts of every character are gathered per language and kind, both sets apart.
`kind` picks the part, onset for an initial and vowel with coda for a rime, off each row's stored anatomy.
A character absent from `readings` adds nothing, and an empty part or blank language records nothing.
Languages sort by their place in the pack's declared `order`, undeclared ones after by name.
Within a language, kinds sort by the declared kind order, undeclared ones after by name.
Places come from `LReflexOrder.LReflexOrderFind`, so the tally and the entry's reflex list order a pack alike.
Names match exactly, never with case folded.
Row languages and kinds are written from the same pack's rules, so a declared name is spelled the same.
So the order the anchored rows were stored in never reaches the page.
A line whose two sets are both empty is left out, and each line lists its marks by count.

## `private static void LTallyLineAdd(Dictionary<(string, string), Dictionary<string, List<string>>> marks, (string, string) key, string text, string character)`

Records one character under one language, kind and part, once however many readings take it.
