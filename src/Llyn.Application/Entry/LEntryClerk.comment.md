# LEntryClerk.cs
Hash: `eda97c4e86297a83`

## `public sealed class LEntryClerk`

The interactor over the entry lifecycle the engine does not own itself.
An Entry is created, read, loaded back as a draft and deleted with the history that leaves.
Searching lives in `LEntryQueryClerk` and the grasp in `LGraspClerk`, since neither is part of the lifecycle.
It runs over the ports of one rig and holds no gate, no observer and no cache.
The engine calls it under its own gate and raises the bulletin a save or a delete deserves.
The delete is the shape every spanning operation here follows.
That shape is several stores, one session, one decision.
The commit of a draft into an entry is here too.
It is composed out of the clerks for each part of an entry.

## `public LEntryClerk(LRig rig, LCardClerk cards, LMeaningClerk meanings, LVocabularyClerk vocabulary, LInflectionClerk inflections, LParadigmClerk paradigms, LPronunciationClerk pronunciations, LTranscriptionClerk transcriptions, LReflexClerk reflexes, LRecordingClerk recordings, LFrequencyClerk frequencies, LRevisionClerk revisions)`

Reads the ports the lifecycle touches out of `rig` and takes the frequency clerk for clearing a renamed entry.
The ports are the root, the entries, the etymologies, the notes and the tombstones.
The revision clerk handed in records every revision and moves the workspace row onto it.
The eight part clerks handed in write the parts of an entry a commit spans.
The recording clerk resolves the recording paths of a loaded draft.

## `public LEntry? LEntryClerkRead(long id)`

Reads the entry for `id`, or `null` when no entry has that id.

## `public LEntryDraft? LEntryClerkLoad(long id)`

The entry as a draft, or `null` when no entry has that id.
Every recording path is made absolute, so a form can play it and a draft match compares like with like.
Its reflex rows come in the order the pack declares, through `LReflexClerkSort`.
The entry facade, citation clerk, outcome clerk, markup intake and portrait load through here.
So none of them sees storage order.

## `public LRevision LEntryClerkDelete(long id)`

Deletes the entry identified by `id` with everything it owns.
It then writes the history the deletion leaves behind.
That is a revision carrying one delete change for the entry.
It is also a tombstone filed under that revision, and the workspace row moved onto it.
Returns the recorded revision.

The delete runs first.
So a refused delete records no history at all and throws its own message through.
A link from another entry does not refuse the delete.
The schema deletes the linking row with the entry.

All four writes share one session, so they are one transaction.
Either all land or none of them do.
Without it a failure part-way would leave an entry deleted with no tombstone naming it.
That is history that no longer describes the file.

## `public LEntry LEntryClerkSave(LEntryDraft draft, Dictionary<long, long> identity)`

Saves the whole input form as one new entry.
That is the headword row with its lexical unit, and its meanings and collocations in card order.
It is also its note, its inflections and its pronunciations when they carry anything.
It is also its etymology, written in whichever of the two shapes the draft ended in.
The note is stored as Markdown normalized by `LMarkdown`.
It is also the revision recording the create, and the workspace row is moved onto that revision.
That revision holds the entry's create first and then a change for every child row written.
Each child is written through the same clerk and seam an update uses, onto an entry with nothing stored.
So a create records exactly what an update adding the same rows to an empty entry would.
Returns the stored entry with its assigned id and timestamps.
A blank headword is refused before any session opens.
The outcome clerk normalizes the draft and trims the headword before handing it over.

Everything runs inside one session, which makes a half-written entry impossible.
A failure at any write rolls back every write before it, leaving no entry row behind.

A card with every field blank writes nothing.
The form keeps an empty card on screen to type into and refuses to remove a list's last card.
Deciding that a blank one is neither belongs here, not in the form.

The entry row is written bare, and its parts of speech and forms follow through the update seams.
Text naming a preset the entry's language declares is stored as that preset's stable id.
Text naming no preset is stored as typed, because the field is editable.

A positive pronunciation, transcription or reflex id the draft still holds names a row of an entry since deleted.
It is reset to zero first, so the commit writes the row anew instead of refusing it.
A negative id is kept, because the identity map records what it became.

## `public LEntry LEntryClerkUpdate(long id, LEntryDraft draft, Dictionary<long, long> identity)`

The save of `draft` onto entry `id` with its revision recorded, in one session.
A save that changed nothing records no revision.

## `public LEntry LEntryClerkSave(long id, LEntryDraft draft, Dictionary<long, long> identity, List<LRevisionDelta> changes)`

Applies `draft` to the entry identified by `id` and returns the stored entry as it now stands.
A blank headword is refused before any session opens, and the headword is trimmed.
The entry keeps its opaque id and its `added_utc`.
Only `updated_utc` moves, and only when the draft differs from the entry as loaded with its recordings resolved.
The headword, language and lexical unit are written together, only when the draft differs.
A changed headword or language clears stored frequencies through the clerk.
No stale figure then appears under the new headword.
An id no entry carries is refused before anything is written.

Every change is appended to `changes` and no revision is recorded here.
The update records the revision once, and the markup import fills many entries under one revision through the same seam.

Which stored card a draft card is comes from `LCardDraft.LCardDraftId`, never from its place in the list.
The meaning clerk reconciles the tree and the card clerk the flat list.
Every part is compared before it is written.
That is the forms, the parts of speech, the inflections, the note, the pronunciations, the transcriptions and the reflexes.
A field that did not change writes no row and records no revision change.
The etymology is written after the rest, and the paradigms are refreshed last.

## `private static void LHeadwordValidate(LEntryDraft draft)`

Refuses a draft whose headword is blank, the one refusal both saves share.
