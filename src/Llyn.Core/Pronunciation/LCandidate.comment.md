# LCandidate.cs

## `public sealed record LCandidate(string LCandidateSource, string LCandidatePhonetic, int LCandidateOrder);`

One pronunciation candidate returned by a lookup source.

**Parameters**

- `LCandidateSource` — The name of the source the candidate came from.
  It is the name that source's language pack declares (for example `"Cambridge"`).
  Source identity is a config-driven name, never a compile-time enum, so the lookup stays language-agnostic.
- `LCandidatePhonetic` — The bare phonetic form, without enclosing brackets.
- `LCandidateOrder` — The source's position in the pack's `pronunciation` list.
  Sources answer at whatever speed the network gives them, so arrival order is meaningless to the reader.
  The position travels with the candidate, so the menu can show the order the pack declares.
