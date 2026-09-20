# LInflectionClerk.cs

## `public sealed class LInflectionClerk`

The clerk over the inflected forms an Entry owns.
Each carries the ordered grammatical features that say what form it is.
Unlike a Tag or an Example, an inflection is not independent data.
It belongs to its Entry and is identified by its place in that Entry's list.
That is why these seams are keyed by the Entry and a position, not by an id of their own.

Writing the whole list and appending to it are separate seams because they answer different questions.
The set makes the stored list the list it is handed, which is what an editor showing every form does.
The append adds to the end without reading what is there, which is what a source contributing forms does.

The clerk deletes the Entry's lacuna rows when a hand edit invalidates them.
Cancelling the fetch that may still be filling them is the engine's, since the engine owns the task.

## `public LInflectionClerk(LRig rig, LParadigmClerk paradigms)`

Reads the entry, inflection, lacuna, morphology and speech ports out of `rig`.
The paradigm clerk judges the regular flag after every write.

## `public IReadOnlyList<LInflection> LInflectionClerkRead(long entryId)`

Reads the inflected forms of the Entry identified by `entryId`, in stored order, each with its features in theirs.

## `public void LInflectionClerkSet(long entryId, IReadOnlyList<LInflection> inflections)`

Makes the stored inflections of the Entry identified by `entryId` exactly `inflections`.
They are stored in the order given, and an empty list clears them.
The regular flags are judged again, the lacuna rows are deleted and the Entry's updated stamp moves.

## `public void LInflectionClerkAppend(long entryId, IReadOnlyList<LInflection> inflections)`

Appends `inflections` to the end of the Entry's list, leaving the forms already stored where they are.

## `public void LInflectionClerkValidate(IReadOnlyList<LInflection> inflections)`

Refuses a list naming a part of speech or a morphology value the workspace no longer holds.
The store would refuse it as a foreign-key failure.
That reaches the reader as a crash and not as an answer.
Every other link a draft carries is checked before it is written, and an inflection is no different.

## `public void LInflectionClerkMove(long entryId, int position, int target)`

Moves the inflection at `position` in the Entry's list to `target`, renumbering the list so the positions stay contiguous.

## `public void LInflectionClerkDelete(long entryId, int position)`

Deletes the inflection at `position` in the Entry's list with its features.
It renumbers the forms after it so the list stays contiguous.
The Entry's lacuna rows are deleted, as a set does.

## `public void LInflectionClerkReset(long entryId)`

Deletes the Entry's lacuna rows, so a form the web could not name before may be asked again.

## `public void LInflectionClerkUpdate(long entryId, LEntryDraft draft, List<LRevisionChange> changes)`

The inflections of the entry as the draft holds them, features and all.
A feature list that differs at one place is a different inflection, so the whole set is rewritten.
An unchanged set writes no row and records no change.

## `public static bool LInflectionClerkMatch(IReadOnlyList<LInflection> stored, IReadOnlyList<LInflection> current)`

Whether two lists hold the same forms with the same features in the same order.
The engine's draft match compares a held draft to its origin through it.
