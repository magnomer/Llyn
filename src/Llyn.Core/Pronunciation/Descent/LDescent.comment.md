# LDescent.cs
Hash: `e79f6eb5541e2a1b`

## `public sealed record LDescent(IReadOnlyList<string> LDescentLanguages, IReadOnlyDictionary<string, IReadOnlyList<string>> LDescentClasses)`

The tone correspondence of one borrowing language.
It names which rime-book tone classes a contour of that language descends from.
Mandarin `55` descends from class 1 and, for a checked syllable, class 7.
Cantonese `13` descends from 4S or 4, since both fell together there.
The Classical Chinese pack declares one row per reflex language in `anatomy_tone.json`, beside `anatomy.json`.
The classes named are the class keys of the pack's own [LHypothesisTone](../Hypothesis/LHypothesisTone.comment.md) rows.
The engine hands the rows to a view, and the view marks the placements whose class the contour allows.

**Parameters**

- `LDescentLanguages` — The borrowing languages the row serves, such as `Mandarin`.
- `LDescentClasses` — Each contour as the anatomy cuts it, such as `214`, to the classes it may descend from.

## `private const char LDescentJoiner = '-';`

The joiner the anatomy keeps between a citation tone and its sandhi form, as `214-21`.

## `public bool LDescentMatch(string language)`

Whether the row serves the language, compared without case.

## `public IReadOnlyList<string> LDescentResolve(string tone)`

The classes the contour may descend from, or none for a blank or unlisted contour.
Only the citation tone before the joiner is looked up, since that is the tone the character carries alone.

## `public static IReadOnlyList<string> LDescentScan(IReadOnlyList<LDescent> rules, string language, string tone)`

The classes under the first row that serves the language, or none when no row does.
