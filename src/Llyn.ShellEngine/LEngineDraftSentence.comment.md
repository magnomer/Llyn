# LEngineDraftSentence.cs

## `public sealed partial class LEngine`

The sentence rows of a card, turned into the stored Example each one names.
An Example is pool data, so an edit made through one card must not rewrite what other cards quote.
The Mentions ride with the Example, matched back into the identity map by offset.
The card sync in `LEngineCard.cs` calls in here for every sentence row it writes.

## Inline notes

### `private static IEnumerable<LSentenceDraft> LEngineSentenceRead(IReadOnlyList<LSentenceDraft> drafts)`

The rows worth writing, in the order the card holds them.
A row stating a frame and no sentence is kept, because the frame is the card's own.
The positions stay a gapless 0, 1, 2 over what is actually stored.

### `private LExample? LEngineExampleResolve(`

The Example a row names, or `null` when the row names none.
A row whose Example carries neither an id nor a sentence names none.
An Example carrying an id names a stored row, however little its sentence says.

### `private LExample LEngineExampleResolve(`

The stored Example a row's positive id names, updated to what the row now says.
A positive id nothing is stored under is refused.
The card would otherwise be bound to a row the user never chose.
An Example other cards also quote is pool data.
An edit made through this card gives this card a fresh row instead.
The other cards keep the row they quoted, unchanged, because nobody edited it there.
A row carrying a negative id gets a fresh Example, recorded in the map under the negative id it replaces.
No row is ever matched by its wording, so two new rows with one sentence stay two rows.
The language falls back to the entry's when the Example states none of its own.
The Mentions count as part of what the row says.
A row whose text and Source are unchanged but whose Mentions differ rewrites the Mention rows in place.
That holds whoever else quotes the Example.
A Mention is the Example's own, so every card citing one sentence reads the same words the same way.
A fork made for a text or Source edit is created with the row's Mentions, already shifted by the edit.
The old row keeps its own.
A fresh Example is created with its Mentions in one call.
Every Mention written under a negative id is recorded in the map, matched by its offset.

### `private static IReadOnlyList<LMentionDraft> LEngineMentionResolve(LExampleDraft written)`

The Mentions of a row that still fit its text, sorted by offset.
A span past the end is dropped here rather than refused.
A text set without the shift therefore cannot block a commit.

### `private static void LEngineMentionRecord(`

Records every negative Mention id under the stored id at the same offset.

### `private bool LEngineShareCheck(long exampleId, long ownerId, bool collocation)`

Whether any card other than `ownerId` quotes the Example.
The owner side matters, because a Meaning and a Collocation can carry one id each.
