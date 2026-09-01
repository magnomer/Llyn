# LCandidate.cs

## `public sealed record LCandidate(string LCandidateSource, string LCandidatePhonetic);`

One pronunciation candidate returned by a lookup source.

**Parameters**

- `LCandidateSource` — The name of the source the candidate came from, as declared by that source's language pack (for example `"Cambridge"`). Source identity is a config-driven name, never a compile-time enum, so the lookup stays language-agnostic.
- `LCandidatePhonetic` — The bare phonetic form, without enclosing brackets.
