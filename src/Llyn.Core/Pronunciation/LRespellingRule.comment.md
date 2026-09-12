# LRespellingRule.cs

## `public sealed record LRespellingRule(string LRespellingRulePattern, string LRespellingRuleReplacement)`

One rewrite step a language pack declares, a regex pattern and the text that replaces every match.
The engine knows nothing of the symbols involved, only that a pattern becomes a replacement.
Every symbol lives in the pack, so a convention changes without a recompile.

**Parameters**

- `LRespellingRulePattern` — A .NET regex matched against the whole transcription.
- `LRespellingRuleReplacement` — A .NET replacement string, so `$1` may echo a captured group.

The pattern is compiled once when the record is built, culture-invariant so no locale bends a class.
An invalid pattern throws there, so a bad pack fails while it loads rather than during a lookup.
The compiled form is a private field, so it never enters the record's equality.

## `public string LRespellingRuleResolve(string phonetic)`

Runs the rule over the whole string and returns the rewritten form.

## `public bool Equals(LRespellingRule? other)`

Two rules are equal when their pattern and replacement text match ordinally.
The synthesized record equality would compare the compiled regex by reference and call twins unequal.

## `public override int GetHashCode()`

Hashes the same two strings the equality reads.
