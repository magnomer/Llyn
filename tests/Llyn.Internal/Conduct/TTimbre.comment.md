# TTimbre.cs

## `public sealed class TTimbre`

Covers the sound facts an editor reads for its held draft, over a fake phonology port.
A phonemic respelling needs a respelling pack first, and a silent pack is not spoken.
An empty desk asks the pack for no language.

## `private static CTimbre TTimbrePrepare(Dictionary<string, Func<object?[]?, object?>> answers)`

Builds an editor over stub ports and a phonology port answering `answers`, and hands back its sound facts.
