# LReflexClerkEpithet.cs
Hash: `8095434e215e4be7`

## `public static class LReflexClerkEpithet`

The epithet of an entry derived from its reflex rows under the rules of its language.

## `public static bool LReflexEpithetCheck(IReadOnlyList<LReflexRule> rules)`

Whether any rule declares an epithet pattern.

## `public static string LReflexEpithetFormat(IReadOnlyList<LReflexRule> rules, IReadOnlyList<LReflexDraft> rows)`

The epithet pieces of the rows, joined, each row formatted by every epithet rule of its language.
Pieces follow the order of `rows`, never the order of the rules.
The caller passes rows sorted by `LReflexClerk.LReflexClerkSort`, so the epithet reads in the pack's declared order.

## `private static string LReflexEpithetFormat(LReflexRule rule, LReflexDraft row)`

One piece with the text, kind, romanization, meaning and note placed, the clip pattern removed and whitespace folded.
A clip pattern that fails to compile or times out leaves the piece unclipped.
