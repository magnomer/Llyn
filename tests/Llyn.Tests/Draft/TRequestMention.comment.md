# TRequestMention.cs

## `public sealed class TRequestMention`

Covers the Mention requests and what commit makes of them.
A word is linked on a card row and on the corpus panel through the same three requests.
A span overlapping a Mention replaces it, and a Meaning of another Entry is refused.
A text edit shifts, keeps or drops the Mentions by where the edit fell.
Commit writes the rows, maps the minted ids, edits a shared row in place for a Mention-only change and forks it for a text change.
The draft file round-trips a minted negative id and a Mention standing for nothing.
The sequences the editor's four gestures send are run as the editor sends them.
A selection past a surrogate pair carries code-point offsets, link then choose then unlink leaves nothing.
Silence then choose is refused, and a word linked in the scribe is found at its offset once committed.

## Inline notes

### `LDraft edited = TMentionTextApply(engine, started.LDraftId, card, sentence, "then she knelt to candle the damp logs again");`

Rewriting the letters inside a word drops its Mention and leaves the one after it where it was.
Only a change of length moves the later spans.
