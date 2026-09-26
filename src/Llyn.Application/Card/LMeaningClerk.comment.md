# LMeaningClerk.cs

## `public sealed class LMeaningClerk`

The clerk over Meanings, the card kind that has a shape.
A Collocation list is flat and is reconciled by the card clerk.
A Meaning list is a tree, because `sense.sense_parent` names a Meaning in the same entry.
So the same three questions are asked once per sibling group rather than once per entry.
Those are which stored row each card is and what to do with the rows no card names.
The third is what order they sit in.
It runs over the meaning port of one rig and hands each card's lines to the card clerk.

## `public LMeaningClerk(LRig rig, LCardClerk cards)`

Reads the meaning port out of `rig` and keeps the card clerk that writes a card's lines.

## `public IReadOnlyList<LMeaning> LMeaningClerkScan(long entryId)`

Reads the Meanings of the Entry identified by `entryId`, in stored order, roots and children together.

## `public void LMeaningClerkCreate(long entryId, long? parentId, LCardDraft card, string language, Dictionary<long, long> identity)`

Writes one Meaning and then the Meanings nested under it, parent before child.
A child needs its parent's id, which only exists once the parent row is written.
Each is appended within its own sibling group, so card order becomes stored position.
An empty child is skipped on the same terms an empty card is.

## `public void LMeaningClerkSave(long entryId, IReadOnlyList<LCardDraft> cards, string language, List<LRevisionDelta> changes, Dictionary<long, long> identity)`

Reconciles the whole Meaning tree of one entry to the cards the draft holds.
Every stored sense of the entry is read once, roots and children together.
The draft is walked whole before anything is deleted, so a card nested three deep still names its row.
Deleting a parent takes its children with it, because the store cascades on `sense_parent`.
So a child of a dropped parent is marked gone even when the draft still names it.
That card is then written as a new row rather than onto a row that no longer exists.
The stored rows arrive parents first, so a parent is always marked before its children are read.

## `private void LMeaningClerkApply(long entryId, long? parentId, IReadOnlyList<LCardDraft> cards, string language, List<LRevisionDelta> changes, IReadOnlyDictionary<long, LMeaning> stored, ISet<long> gone, ISet<long> applied, Dictionary<long, long> identity)`

Writes one sibling group and then, for each card in it, the group nested under that card.
A card naming a stored row updates that row and keeps its id.
A card that moved to another parent moves its stored row with it.
The row is not deleted and written again, because its id is what every example and link hangs from.
A card naming nothing, or naming a row already gone, creates one under this parent.
A card names a stored row once, across the whole tree and not merely within one group.
The whole group is renumbered afterwards in one pass.
The unique sibling index rejects a swap done row by row.
That is what `LMeaningOrderSet` on the meaning vault exists for.

## `private static void LMeaningClerkScan(IReadOnlyList<LCardDraft> cards, IReadOnlyDictionary<long, LMeaning> stored, ISet<long> named)`

Every stored row the draft still names, wherever in the tree it names it.
The deletion pass needs the whole answer before it removes anything.
A card that has gone blank names nothing, so clearing a card removes its row and its children.
