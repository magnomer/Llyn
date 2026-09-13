# LParadigmRule.cs

## `public sealed record LParadigmRule(string LParadigmRulePattern, string LParadigmRuleReplacement)`

One rule predicting a regular spelling of an inflected form from the headword.
The pattern is a regular expression, and the replacement is what its first match becomes.
A headword the pattern does not match yields no prediction from this rule.
So `([^aeiou])y$` with `$1ies` turns `city` into `cities` and leaves `day` alone.
Several rules may match one headword, and every prediction they make counts as regular.
Matching ignores case and waits at most one second.
Equality compares the two strings, so a pack reloaded with the same rules compares equal.

**Parameters**

- `LParadigmRulePattern` — Regular expression matched against the headword.
- `LParadigmRuleReplacement` — Replacement text for the first match, with `$1` style group references.

## `public string? LParadigmRuleResolve(string headword)`

Predicts the regular spelling for `headword`, or `null` when the pattern does not match it.
Only the first match is replaced, so an end anchor never fires twice.
A match that times out predicts nothing rather than failing the reader.
