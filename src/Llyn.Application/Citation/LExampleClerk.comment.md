# LExampleClerk.cs

## `public sealed class LExampleClerk`

The clerk over the Examples of the workspace, and over the Example a sentence row names.
An Example is pool data owned by nothing.
So an edit made through one card must not rewrite what other cards quote.
Both card sides may reference the same one, each holding its own order over the Examples it references.
Two sides is why the side arrives as an `LOwner` and not in the method's name.
The Mentions ride with the Example, matched back into the identity map by offset.
The card clerk calls in here for every sentence row it writes.
The corpus panel reads its rows and its page here, so screen and print show one thing.

## `public LExampleClerk(LRig rig, LReferenceClerk references)`

Reads the example, gloss and mention ports out of `rig`.
The reference clerk names the Source an Example cites.

## `public static LExample LExampleClerkBlank`

What a draft naming no Example is measured against.

## `public LExample LExampleClerkCreate(LExample example)`

Creates `example` and returns it with its assigned id.

## `public LExample? LExampleClerkRead(long id)`

Reads the Example for `id`, or `null` when no Example has that id.

## `public LPortraitPage? LExampleClerkRead(long id, LPortraitLegend legend)`

The page of one Example, or null when no Example has that id.
The cited Source is read for it, and the usage tally is counted from the rows quoting it.

## `public IReadOnlyList<LCatalogExample> LExampleClerkFind(string query, LCatalogOrder order)`

The Examples answering `query`, in `order`, as rows already carrying their cited name and quotation count.
An Example is matched over its sentence, its translation and the name of the Source it cites.
The name is resolved here, because matching on an id the reader never sees would answer the wrong question.

## `public void LExampleClerkUpdate(LExample example)`

Rewrites the language, the sentence and the translation of the Example `example` identifies.

## `public void LExampleClerkDelete(long id, bool detach)`

Deletes the Example, dropping every reference to it first when the user asked for that.

## `public static bool LExampleClerkMatch(LExample one, LExample other)`

Field by field, whether two sentences say the same thing.
Identity is left out, because a held sentence is named before the Example it becomes exists.
The language counts, because the tongue a sentence is written in is part of the sentence.
The cited Source counts too, and so do the Mentions.

## `public static LPortraitPage LExamplePageRead(LExample example, LReference? cited, int count, LPortraitLegend legend)`

The sentence heads the page, its language is the chip, and its usage tally follows.
Each gloss is a line labelled with its language, under the translation heading.
The cited source is named as the catalog names it, and an example citing nothing shows no source.

## `private static string LExampleSourceRead(IReadOnlyDictionary<long, string> named, LStateAnchor source)`

The cited name a catalog row shows, the bare id when the Source is gone, nothing when none is cited.

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
