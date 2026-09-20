# LEntryClerk.cs

## `public sealed class LEntryClerk`

The interactor over the entry lifecycle the engine does not own itself.
An Entry is created, read, searched for, loaded back as a draft and deleted with the history that leaves.
It runs over the ports of one rig and holds no gate, no observer and no cache.
The engine calls it under its own gate and raises the bulletin a save or a delete deserves.
The delete is the shape every spanning operation here follows.
That shape is several stores, one session, one decision.
The commit of a draft into an entry is here too.
It is composed out of the clerks for each part of an entry.
The transcriptions and the reflexes are still reconciled by the engine after the clerk returns.
Their sync reads the source factory, which no clerk holds until plan 16 gives it a clerk.
The engine holds the outer session across both halves, so a commit is still whole or nothing.

## `public LEntryClerk(LRig rig, LCardClerk cards, LMeaningClerk meanings, LVocabularyClerk vocabulary, LInflectionClerk inflections, LParadigmClerk paradigms, LPronunciationClerk pronunciations)`

Reads the ports the lifecycle touches out of `rig`.
Those are the root, the entries, the frequencies, the notes, the revisions, the tombstones and the workspace row.
The six clerks handed in write the parts of an entry a commit spans.

## `public LEntry LEntryClerkCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)`

Creates `entry` with `forms` and `speeches` as its ordered child rows.
Returns the stored entry with its assigned id and timestamps.

## `public LEntry? LEntryClerkRead(long id)`

Reads the entry for `id`, or `null` when no entry has that id.

## `public LEntryDraft? LEntryClerkLoad(long id)`

The stored entry as the input form would have handed it over, or `null` when no entry has that id.
Every string comes out as stored, and nothing is derived on the way out.
A recording comes out relative to the workspace, and the engine resolves it, since only it knows the folder.

## `public IReadOnlyList<LEntry> LEntryClerkFind(string query)`

Returns the entries whose headword contains `query`, ordered by headword.
It returns every entry when `query` is empty or all whitespace.
Matching is a contains whose case is folded over the whole of Unicode.
So an accented headword is found typed in either case.

## `public IReadOnlyList<LEntry> LEntryClerkFind(string query, LCatalogOrder order)`

The entries answering `query`, in `order`.
The store answers which entries match, and the ordering is applied over what it returned.

## `public IReadOnlyList<LEntry> LEntryClerkFind(string query, LCatalogOrder order, LCatalogFilter filter)`

The ordered search with the entries in a hidden language left out.
The library panel lists through this, so the filter is applied here and never in the shell.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LTag tag)`

Returns the entries carrying `tag`, matched by its id, ordered by headword.
A zero id stands for no tag chosen and returns every entry.
The overload takes a tag rather than text so the two searches cannot be confused.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LTag tag, LCatalogFilter filter)`

The entries carrying `tag` with those in a hidden language left out.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LTag tag, string query, LCatalogFilter filter)`

The filtered entries carrying `tag` narrowed to those whose headword matches `query`.
The match is the catalog match, so `*` and `?` work as in every search field.
An empty or blank `query` narrows nothing.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LRegister register)`

Returns the entries marked with `register`, matched by its id, ordered by headword.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LRegister register, LCatalogFilter filter)`

The entries marked with `register` with those in a hidden language left out.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LRegister register, string query, LCatalogFilter filter)`

The filtered entries marked with `register` narrowed to those whose headword matches `query`.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LSituation situation)`

Returns the entries referencing `situation`, matched by its id, ordered by headword.
A zero id stands for no situation chosen and returns every entry.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LSituation situation, string query, LCatalogFilter filter)`

The entries referencing `situation`, those in a hidden language left out, narrowed to headwords matching `query`.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LExample example)`

Returns the entries quoting `example`, matched by its id, ordered by headword.
A zero id stands for no example chosen and returns every entry.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LExample example, string query, LCatalogFilter filter)`

The entries quoting `example`, those in a hidden language left out, narrowed to headwords matching `query`.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LReference reference)`

Returns the entries citing `reference`, matched by its id, ordered by headword.
A card cites a Source through the Example it holds, so the walk goes through that Example.
A zero id stands for no source chosen and returns every entry.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LReference reference, string query, LCatalogFilter filter)`

The entries citing `reference`, those in a hidden language left out, narrowed to headwords matching `query`.

## `public static IReadOnlyList<LEntry> LEntryClerkMatch(IReadOnlyList<LEntry> entries, string query)`

The entries whose headword matches `query` by the catalog match.
An empty or blank `query` narrows nothing.
The engine's vista narrows a child list the same way, so it calls here.

## `public LRevision LEntryClerkDelete(long id)`

Deletes the entry identified by `id` with everything it owns.
It then writes the history the deletion leaves behind.
That is a revision carrying one delete change for the entry.
It is also a tombstone filed under that revision, and the workspace row moved onto it.
Returns the recorded revision.

The delete runs first.
So a refused delete records no history at all and throws its own message through.
A delete is refused when another entry still links to the entry.

All four writes share one session, so they are one transaction.
Either all land or none of them do.
Without it a failure part-way would leave an entry deleted with no tombstone naming it.
That is history that no longer describes the file.

## `public void LEntryGraspSet(long entryId, int grasp)`

Writes the user's grasp onto the entry.
The range is checked before any write, so a bad value reaches neither the store nor a subscriber.
No revision is recorded, because a rating is a reading mark and not an edit of the word.

## `public string LEntryEpithetRead(long entryId)`

The epithet stored on the entry, or empty for an id no entry carries.

## `public void LEntryEpithetSave(long entryId, string epithet)`

Writes the derived epithet onto the entry.

## `public IReadOnlyDictionary<long, string> LEntryEpithetScan(IReadOnlyList<long> ids)`

The epithet of every listed entry that has one, keyed by id, in one statement.

## `public long LEntryCountRead()`

How many entries the workspace holds.

## `public long LWorkspaceSizeRead()`

The bytes of the main database file, for the status bar.

## `public LTombstone? LTombstoneRead(long entryId)`

Reads the tombstone left by deleting the Entry identified by `entryId`, or `null` when that Entry has never been deleted.

## `public LRevision? LRevisionRead()`

Reads the revision the workspace row points at, or `null` when the workspace has none yet.
The pointer is what every save and delete moves.
The current revision is read from where it is recorded rather than guessed from the newest row.

## `public IReadOnlyList<LRevisionChange> LRevisionChangeRead(long revisionId)`

Reads the changes recorded under `revisionId`, in the order recorded.

## `public LEntry LEntryClerkSave(LEntryDraft draft, Dictionary<long, long> identity)`

Saves the whole input form as one new entry.
That is the headword row and its meanings and collocations in card order.
It is also its note, its inflections and its pronunciations when they carry anything.
The note is stored as Markdown normalized by `LMarkdown`.
It is also the revision recording the create, and the workspace row is moved onto that revision.
Returns the stored entry with its assigned id and timestamps.
The engine normalizes the draft and trims the headword before handing it over.

Everything runs inside one session, which makes a half-written entry impossible.
A failure at any write rolls back every write before it, leaving no entry row behind.

A card with every field blank writes nothing.
The form keeps an empty card on screen to type into and refuses to remove a list's last card.
Deciding that a blank one is neither belongs here, not in the form.

The part of speech is written with the entry rather than after it, as its owned child row.
Text naming a preset the entry's language declares is stored as that preset's stable id.
Text naming no preset is stored as typed, because the field is editable.

A positive pronunciation id the draft still holds names a row of an entry since deleted.
It is reset to zero first, so the commit writes the row anew instead of refusing it.
A negative id is kept, because the identity map records what it became.

## `public LEntry LEntryClerkSave(long id, LEntryDraft draft, bool changed, Dictionary<long, long> identity, List<LRevisionChange> changes)`

Applies `draft` to the entry identified by `id` and returns the stored entry as it now stands.
The entry keeps its opaque id and its `added_utc`.
Only `updated_utc` moves, and only when `changed` says the draft differs from the entry as loaded.
The engine judges that with its draft match, since only it can load the entry as the form saw it.
A changed headword or language clears the stored frequency, so a stale figure never shows under the new headword.
An id no entry carries is refused before anything is written.

Every change is appended to `changes` and no revision is recorded here.
The engine records the revision once the parts it still reconciles have added theirs.
The markup import fills many entries under one revision through the same seam.

Which stored card a draft card is comes from `LCardDraft.LCardDraftId`, never from its place in the list.
The meaning clerk reconciles the tree and the card clerk the flat list.
Forms, parts of speech, inflections, the note and the pronunciations are each compared before they are written.
A field that did not change writes no row and records no revision change.
The lacuna rows go, because a hand edit is a reason to ask the web again.
The regular flags are judged last, since the judgement reads the headword, the parts and the forms.

## `public LRevision LRevisionRecord(IReadOnlyList<LRevisionChange> changes)`

Records one revision holding `changes` and points the workspace row at it.
Every save and delete moves that pointer, so the current revision is read from where it is recorded.

## `public void LEntryUpdatedSet(long entryId)`

Moves the entry's `updated_utc` to now.
Every seam that changes one part of an entry outside the draft path calls this.
Otherwise the stamp would say the entry stood still while a card or a pronunciation moved.

## `private static IReadOnlyList<LPronunciationDraft> LPronunciationReset(IReadOnlyList<LPronunciationDraft> drafts)`

Every pronunciation row of the draft as a row still to be created.

## `private static void LFormUpdate(LEntryVault entries, long entryId, LEntryDraft draft, List<LRevisionChange> changes)`

The forms of the entry as the draft holds them, replacing what stood before.
Positions are rewritten by the archive, so a reordered list stores as the new order.

## `public static bool LFormMatch(IReadOnlyList<LForm> stored, IReadOnlyList<LForm> current)`

Whether two form lists hold the same text, role and label in the same order.
Those three are what a form is.
The engine's draft match compares a held draft to its origin through it.

## `private void LNoteUpdate(long entryId, LEntryDraft draft, List<LRevisionChange> changes)`

The entry's note reconciled to the draft.
The text is normalized Markdown, brought to canonical form by `LMarkdown` before it is compared or stored.
A note the user cleared is deleted rather than left standing as their last words.
