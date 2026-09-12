# LMentionResult.cs

## `public sealed record LMentionResult(`

The engine's answer for a clicked word of a sentence.
It names the span the click resolves to and either the Mention already stored on it or the Entries the word may mean.
A stored Mention comes with an empty candidate list, because the word is already decided.
A word with no Mention comes with every Entry of the language whose headword is that word, in headword order.
Zero candidates is a legitimate answer, and the form shows nothing for it.
A click on a space or a mark answers a span of length zero.

**Parameters**

- `LMentionResultOffset` — The code-point offset the resolved span begins at.
- `LMentionResultLength` — The number of code points the span occupies, zero when the click hit no word.
- `LMentionResultStored` — The Mention stored on the span, or `null` when none covers the click.
- `LMentionResultEntry` — The Entries whose headword is the word, empty when a Mention is stored or nothing matches.
