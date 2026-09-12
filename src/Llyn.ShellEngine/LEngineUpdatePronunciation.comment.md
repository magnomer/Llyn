# LEngineUpdatePronunciation.cs

## `public sealed partial class LEngine`

The pronunciations of an entry reconciled to its draft.
A draft holds an ordered list of pronunciation rows by id, and a save makes the store say the same.
The same reconciliation serves a create and an update, since a fresh entry is an entry with nothing stored.

## Inline notes

### `private void LEnginePronunciationSync(`

Blank rows are left out, because a row with neither reading nor recording is not a pronunciation.
A positive id naming no stored row of this entry refuses the commit rather than rebinding to a fresh row.
A stored row the draft no longer names is deleted, its recording with it.
Every row kept is rewritten only when it changed, so an unchanged row raises no change.
A new row is appended and then the whole list is placed in draft order.
The order is rewritten only when a row moved or the count changed, so an untouched list writes nothing.

### `private static long LEnginePronunciationSave(`

One kept row brought to the draft's variety, reading and syllables.
Its id and its place survive, so the recording hanging from it stays attached across an edit of the IPA.

### `private static long LEnginePronunciationInsert(`

One new row written after the entry's others, its minted id recorded against the draft id it replaces.

### `private void LEngineAudioSync(`

The recording of one row reconciled to the draft.
The draft carries the recording as a full path and the row stores it relative to the workspace.
So the comparison is made in stored terms.
An unchanged recording writes nothing, and an empty one leaves the stored recording alone.

### `private static bool LEngineSoundMatch(LPronunciation stored, LPronunciation current)`

Whether a stored row and its draft say the same thing.
The syllables are compared without their parent id, since the draft's carry none yet.
