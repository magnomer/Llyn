# LInflectionRule.cs
Hash: `62c7a8c70cf48ea5`

## `public sealed record LInflectionRule(string LInflectionRulePattern, string LInflectionRuleReplacement, string? LInflectionRuleKind) : IEquatable<LInflectionRule>`

One ordered rewrite of a rule book, used both for spelling rules and for comparison folds.
Rules run in book order, and each one replaces every match.
Prediction checks kind restrictions, but this record's rewrite and scan methods do not.
The regex is culture-invariant and case-sensitive by default, with a one-second timeout.
Equality compares the three strings, because the compiled pattern differs per instance.

**Parameters**

- `LInflectionRulePattern`: the regular expression to rewrite.
- `LInflectionRuleReplacement`: replacement text supporting regex group references.
- `LInflectionRuleKind`: the prediction kind restriction, or null for every headword.

## `public string LInflectionRuleResolve(string text)`

Replaces every match in `text`.
A match that times out leaves the text unchanged, so one slow rule never fails the prediction.

## `public IReadOnlyList<Match> LInflectionRuleScan(string text)`

Every match in `text`, for a caller that must track where each replacement came from.
Timeouts discard the entire scan, letting the fold caller preserve this rule's input.

## `public bool Equals(LInflectionRule? other)`

Ordinal equality compares pattern, replacement, and kind without including the cached regex instance.

## `public override int GetHashCode()`

Hashing uses the same three strings as equality.
