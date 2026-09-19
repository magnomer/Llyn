# LEngineMeaningCard.cs

## `public sealed partial class LEngine`

The Meaning half of the card update, which is the half that has a shape.
A Collocation list is flat and is reconciled where the other card kinds are.
A Meaning list is a tree, because `sense.sense_parent` names a Meaning in the same entry.
So the same three questions are asked once per sibling group rather than once per entry.
Those are which stored row each card is and what to do with the rows no card names.
The third is what order they sit in.
A row whose title, definition and place all stand is neither rewritten nor recorded.
An unreadable value is still sent, because the store is what refuses it.

## Inline notes

### `private void LEngineMeaningUpdate(`

Reconciles the whole Meaning tree of one entry to the cards the draft holds.
Every stored sense of the entry is read once, roots and children together.
The draft is walked whole before anything is deleted, so a card nested three deep still names its row.

### `HashSet<string> gone = new(StringComparer.Ordinal);`

The rows that are no longer there once the deletions have run.
Deleting a parent takes its children with it, because the store cascades on `sense_parent`.
So a child of a dropped parent is marked gone even when the draft still names it.
That card is then written as a new row rather than onto a row that no longer exists.
The stored rows arrive parents first, so a parent is always marked before its children are read.

### `private void LEngineMeaningApply(`

Writes one sibling group and then, for each card in it, the group nested under that card.
A card naming a stored row updates that row and keeps its id.
A card naming nothing, or naming a row already gone, creates one under this parent.
The whole group is renumbered afterwards in one pass.
The unique sibling index rejects a swap done row by row.
That is what `LMeaningOrderSet` on the meaning vault exists for.

### `if (!string.Equals(row.LMeaningParentId, parentId, StringComparison.Ordinal))`

A card that moved to another parent moves its stored row with it.
The row is not deleted and written again, because its id is what every example and link hangs from.

### `ISet<string> applied`

A card names a stored row once, across the whole tree and not merely within one group.
Two cards carrying one id would otherwise both write that row.
The renumber would then be handed the same member twice.
So the second is a new card.

### `private static void LEngineMeaningScan(`

Every stored row the draft still names, wherever in the tree it names it.
The deletion pass needs the whole answer before it removes anything.
A card that has gone blank names nothing, so clearing a card removes its row and its children.
