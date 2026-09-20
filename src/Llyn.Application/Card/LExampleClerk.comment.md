# LExampleClerk.cs

## `public sealed class LExampleClerk`

The clerk over the Example a sentence row names.
An Example is pool data, so an edit made through one card must not rewrite what other cards quote.
The Mentions ride with the Example, matched back into the identity map by offset.
The card clerk calls in here for every sentence row it writes.
The reads of the corpus panel still sit in the engine and join this clerk in a later plan.

## `public LExampleClerk(LRig rig)`

Reads the example, gloss and mention ports out of `rig`.

## `public LExample? LExampleClerkResolve(LSentenceDraft draft, string language, long ownerId, bool collocation, Dictionary<long, long> identity)`

The Example a row names, or `null` when the row names none.
A row whose Example carries neither an id nor a sentence names none.
An Example carrying an id names a stored row, however little its sentence says.

## `public LExample LExampleClerkResolve(LExampleDraft written, string language, long ownerId, bool collocation, Dictionary<long, long> identity)`

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
A fork made for a text or Source edit is created with the row's Mentions, already shifted by the edit.
Every Mention written under a negative id is recorded in the map, matched by its offset.

## `public static IReadOnlyList<LMentionDraft> LMentionResolve(LExampleDraft written)`

The Mentions of a row that still fit its text, sorted by offset.
A span past the end is dropped here rather than refused.
A text set without the shift therefore cannot block a commit.

## `private static void LMentionRecord(Dictionary<long, long> identity, IReadOnlyList<LMentionDraft> drafts, IReadOnlyList<LMention> stored)`

Records every negative Mention id under the stored id at the same offset.

## `private bool LExampleShareCheck(long exampleId, long ownerId, bool collocation)`

Whether any card other than `ownerId` quotes the Example.
The owner side matters, because a Meaning and a Collocation can carry one id each.

## `private static void LGlossRecord(Dictionary<long, long> identity, IReadOnlyList<LGlossDraft> drafts, IReadOnlyList<LGloss> stored)`

Records the stored id each new gloss row received, matched by position after a save.
The identity map lets a later request that still names the negative id reach the stored row.
