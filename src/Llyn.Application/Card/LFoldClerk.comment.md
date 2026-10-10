# LFoldClerk.cs
Hash: `9d2f0fde4a8aa1e1`

## `public sealed class LFoldClerk`

Card folds, "More readings" and editor boxes are persistent view state, not lexical edits.
These operations neither change a draft nor record revisions.
Nonpositive ids never reach storage, because fresh cards and entries have no stored fold marks.

## `public LFoldClerk(LRig rig)`

The rig supplies the persistence port, keeping database details outside the clerk.

## `public IReadOnlySet<long> LFoldClerkRead(long entryId)`

One entry read covers folded cards of both kinds.
A nonpositive entry id answers an empty set without reading storage.

## `public void LFoldClerkSave(long cardId)`

Only positive card ids can acquire persistent fold marks.

## `public void LFoldClerkDelete(long cardId)`

Only positive card ids can remove persistent fold marks.

## `public bool LFoldClerkLoad(long entryId)`

"More readings" is opened per entry, with false for a nonpositive id.

## `public void LFoldClerkSpread(long entryId, bool opened)`

Each stored entry keeps its own "More readings" state, independent of other entries.
Nonpositive entry ids write nothing.

## `public bool LFoldClerkLoad(long entryId, LFoldBox box)`

Each editor box is opened per entry, with false for a nonpositive id.

## `public void LFoldClerkSpread(long entryId, LFoldBox box, bool opened)`

The entry and box jointly identify the state, independent of other entries and boxes.
Views of the same entry read the same stored state.
Nonpositive entry ids write nothing.

## `public void LFoldClerkSpread(long entryId, string key, bool opened)`

Opens or closes one series member's "More readings", keyed by its entry and the series key.
It is separate from the entry page's opening, so folding a member leaves the entry page as it was.
`LStemClerk` reads the state back into each member.
Nonpositive entry ids write nothing.
