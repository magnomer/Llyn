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

Reads the example, gloss, mention and sentence ports out of `rig`.
The reference clerk names the Source an Example cites.

## `public static LExample LExampleClerkBlank`

What a draft naming no Example is measured against.

## `public LExample LExampleClerkCreate(LExample example)`

Creates `example` and returns it with its assigned id.

## `public LExample? LExampleClerkRead(long id)`

Reads the Example for `id`, or `null` when no Example has that id.

## `public IReadOnlyList<LExample> LExampleClerkRead()`

Every Example in the workspace, for the panel that browses the shared stock of sentences itself.

## `public IReadOnlyList<LExample> LExampleClerkRead(long ownerId, LOwner owner)`

Reads the Examples the Meaning or Collocation identified by `ownerId` references, in the order that side holds them.
A side no sentence table serves is refused rather than picked for.

## `public LPortraitPage? LExampleClerkRead(long id, LPortraitLegend legend)`

The page of one Example, or null when no Example has that id.
The cited Source is read for it, and the usage tally is counted from the rows quoting it.

## `public IReadOnlyList<LCatalogExample> LExampleClerkFind(string query, LCatalogOrder order)`

The Examples answering `query`, in `order`, as rows already carrying their cited name and quotation count.
An Example is matched over its sentence, its translation and the name of the Source it cites.
The name is resolved here, because matching on an id the reader never sees would answer the wrong question.

## `public IReadOnlyList<LSentence> LSentenceRead(long meaningId)`

The sentence rows one Meaning holds, in its order.

## `public IReadOnlyList<LSentence> LSentenceRead(long ownerId, LOwner owner)`

The sentence rows the Meaning or Collocation identified by `ownerId` holds, in the order that side holds them.

## `public void LExampleClerkUpdate(LExample example)`

Rewrites the language, the sentence and the translation of the Example `example` identifies.

## `public void LExampleClerkUpdate(long exampleId, LStateAnchor reference)`

Sets or clears the single Source the Example identified by `exampleId` cites.
Only the citation moves, and the Reference row itself is neither created, changed nor deleted.

## `public void LExampleClerkAttach(long ownerId, long exampleId, int position, LOwner owner)`

References the Example from the Meaning or Collocation identified by `ownerId`, at `position`.
The set is renumbered around it so the positions stay contiguous.

## `public void LExampleClerkDetach(long ownerId, long exampleId, LOwner owner)`

Removes one side's reference to an Example.
The Example and its other references survive, which is the rule a card edit follows.

## `public void LExampleClerkRemove(long ownerId, long exampleId, LOwner owner)`

Removes one side's reference to an Example and deletes the Example when that was its last reference.
The engine wraps the two in one session, so the row is judged against the references as they stand.

## `public void LExampleClerkDelete(long id)`

Deletes the Example identified by `id`.
Refused while any Meaning or Collocation still references it.

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

## `private static ArgumentOutOfRangeException LExampleOwnerRaise(LOwner owner)`

The refusal for a side no sentence table serves.

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
