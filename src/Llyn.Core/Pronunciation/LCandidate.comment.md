# LCandidate.cs

## `public sealed record LCandidate(string LCandidateSource, string? LCandidatePhonetic, int LCandidateOrder, bool LCandidateReached, string LCandidateVariety)`

What one lookup source had to say about a headword.
Every source produces at least one, so the menu shows a row for each and none is silently absent.
A source that returned several varieties produces one per variety, each carrying its tag.
A source that failed is as much a result as one that answered, and the user is told which happened.

**Parameters**

- `LCandidateSource` — The name of the source the candidate came from.
  It is the name that source's language pack declares (for example `"Cambridge"`).
  Source identity is a config-driven name, never a compile-time enum, so the lookup stays language-agnostic.
- `LCandidatePhonetic` — The bare phonetic form, without enclosing brackets, or null when the source gave none.
- `LCandidateOrder` — The source's position in the pack's `pronunciation` list.
  Sources answer at whatever speed the network gives them, so arrival order is meaningless to the reader.
  The position travels with the candidate, so the menu can show the order the pack declares.
- `LCandidateReached` — Whether the source answered at all.
  With no phonetic form, this is what separates two cases.
  A word the dictionary does not carry is told from a dictionary that is down.
- `LCandidateVariety` — The regional variety the form belongs to, matching one the pack declares.
  It is empty when the source gave no variety, and the menu shows the row untagged.
