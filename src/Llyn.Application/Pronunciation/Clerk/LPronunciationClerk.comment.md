# LPronunciationClerk.cs
Hash: `a55ec74c732f56f4`

## `public sealed class LPronunciationClerk`

The pronunciations of an entry reconciled to its draft.
A draft holds an ordered list of pronunciation rows by id, and a save makes the store say the same.
The same reconciliation serves a create and an update, since a fresh entry is an entry with nothing stored.
It runs over the pronunciation port of one rig and the trail and workspace root beside it.

## `public LPronunciationClerk(LRig rig)`

Reads the entry and pronunciation ports, the trail and the workspace root out of `rig`.

## `public IReadOnlyList<LCatalogPronunciation> LPronunciationClerkFind(string query, LCatalogOrder order)`

The pronunciation catalog: every entry matching `query` with its first reading, sorted.

## `public void LPronunciationClerkSync(long entryId, IReadOnlyList<LPronunciationDraft> drafts, List<LRevisionDelta> changes, Dictionary<long, long> identity)`

Only a seeded row still holding no IPA, respelling, recording or syllables is left out.
The draft seeds that row itself for a first reading, so emptied again it holds nothing written.
A blank row that was not seeded is kept and written.
A positive id naming no stored row of this entry refuses the commit rather than rebinding to a fresh row.
A stored row the draft no longer names is deleted, its recording with it.
Every row kept is rewritten only when it changed, so an unchanged row raises no change.
A new row is appended and then the whole list is placed in draft order.
The order is rewritten only when a row moved or the count changed, so an untouched list writes nothing.
Every row written is recorded on `changes`, on a create as on an update.

## `public static LArticulation LPronunciationConsonantRead()`

The IPA consonant chart a pronunciation is typed from, handed up for every driver's input aid.

## `public static LArticulation LPronunciationVowelRead()`

The IPA vowel chart a pronunciation is typed from, handed up for every driver's input aid.

## `public static IReadOnlyList<LPronunciationDraft> LPronunciationClerkScan(IReadOnlyList<LPronunciationDraft> drafts)`

The rows worth writing, their reading and respelling trimmed.
A seeded row still empty is the blank the form keeps to type into and is dropped.
The engine's draft match filters the same way, so a held draft and its origin compare on the same rows.

## `private long LPronunciationClerkSave(LPronunciation stored, LPronunciationDraft draft, List<LRevisionDelta> changes)`

One kept row brought to the draft's variety, reading, respelling and syllables.
Its id and its place survive, so the recording hanging from it stays attached across an edit of the IPA.

## `private long LPronunciationClerkInsert(long entryId, LPronunciationDraft draft, List<LRevisionDelta> changes, Dictionary<long, long> identity)`

One new row written after the entry's others, its minted id recorded against the draft id it replaces.

## `private void LAudioSync(long pronunciationId, LPronunciationDraft draft, List<LRevisionDelta> changes)`

The recording of one row reconciled to the draft.
The draft carries the recording as a full path and the row stores it relative to the workspace.
So the comparison is made in stored terms.
An unchanged recording writes nothing, and an empty one leaves the stored recording alone.

## `private string LRecordingFormat(string path)`

The recording path made relative to the workspace, so a moved workspace keeps its audio.
A path outside the workspace is stored as given.

## `private static bool LPronunciationClerkMatch(LPronunciation stored, LPronunciation current)`

Whether a stored row and its draft say the same thing.
The syllables are compared without their parent id, since the draft's carry none yet.
