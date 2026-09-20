# LReflexClerkEpithet.cs

## `public static class LReflexClerkEpithet`

The epithet of an entry derived from its reflex rows under the rules of its language.

## `public static bool LReflexEpithetCheck(IReadOnlyList<LReflexRule> rules)`

Whether any rule declares an epithet pattern.

## `public static string LReflexEpithetFormat(IReadOnlyList<LReflexRule> rules, IReadOnlyList<LReflex> rows)`

The epithet pieces of every rule over the rows of its language, joined.

## `private static string LReflexEpithetFormat(LReflexRule rule, LReflex row)`

One piece with the text, kind and note placed, the clip pattern removed and whitespace folded.
A clip pattern that fails to compile or times out leaves the piece unclipped.
