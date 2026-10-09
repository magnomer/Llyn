# LInflectionClerk.cs
Hash: `12e79993ef68b91e`

## `public sealed class LInflectionClerk`

The clerk over the inflected forms an Entry owns.
Each carries the ordered grammatical features that say what form it is.
Unlike a Tag or an Example, an inflection is not independent data.
It belongs to its Entry and is identified by its place in that Entry's list.
That is why these seams are keyed by the Entry, not by an id of their own.

The update makes the stored list the list the draft holds, which is what an editor showing every form does.

The clerk deletes the Entry's lacuna rows when a hand edit invalidates them.
Cancelling the fetch that may still be filling them is the engine's, since the engine owns the task.

## `public LInflectionClerk(LRig rig)`

Reads the inflection, lacuna, morphology and speech ports out of `rig`.
The regular flag is not judged here.
The entry save asks the paradigm clerk for it after its writes.

## `public void LInflectionClerkValidate(IReadOnlyList<LInflection> inflections)`

Refuses a list naming a part of speech or a morphology value the workspace no longer holds.
The store would refuse it as a foreign-key failure.
That reaches the reader as a crash and not as an answer.
Every other link a draft carries is checked before it is written, and an inflection is no different.

## `public void LInflectionClerkReset(long entryId)`

Deletes the Entry's lacuna rows, so a form the web could not name before may be asked again.

## `public void LInflectionClerkUpdate(long entryId, LEntryDraft draft, List<LRevisionDelta> changes)`

The inflections of the entry as the draft holds them, features and all.
A feature list that differs at one place is a different inflection, so the whole set is rewritten.
An unchanged set writes no row and records no change.
A new entry is saved through here too, so its first inflections are recorded as a create.

## `public static bool LInflectionClerkMatch(IReadOnlyList<LInflection> stored, IReadOnlyList<LInflection> current)`

Whether two lists hold the same forms with the same features in the same order.
The engine's draft match compares a held draft to its origin through it.
