# LEngineSense.cs

## `public sealed partial class LEngine`

The Meaning and Collocation halves of the engine: the two card kinds an Entry is made of, reached one row at a time rather than through the whole-form save. The form writes and rewrites a whole draft, which is the only path the input panel needs; these seams are the model's own — create one Meaning, move it among its siblings, delete it — and they exist whether or not a control does.

Both are ordered within their Entry, so both carry a move, and both delete downwards: a Meaning takes its subordinate Meanings and its association rows with it, a Collocation takes its synonym interlinks and its association rows, and the independent Examples, Tags and Situations either referenced are left standing. Only the links go.

A Meaning is refused deletion while a relation from outside the subtree, or any Collocation synonym, still points at it: a link is a statement about a Meaning that exists, and a cascade must not decide on its own to unmake someone else's statement.

## `public LSense LEngineSenseCreate(LSense sense)`

Creates `sense` at the end of its Entry's Meanings — or of its parent's subordinate Meanings — and returns it with its assigned id and position.

## `public LSense? LEngineSenseRead(string id)`

Reads the Meaning for `id`, or `null` when none has that id.

## `public IReadOnlyList<LSense> LEngineSenseRead(string ownerId, LOwner owner)`

Reads the Meanings of the Entry identified by `ownerId`, in stored order.

## `public void LEngineSenseUpdate(LSense sense)`

Rewrites the fields of the Meaning `sense` identifies. Its place among its siblings is not touched here — `LEngineSenseMove` owns the order.

## `public void LEngineSenseMove(string id, int position)`

Moves the Meaning identified by `id` to `position` among its siblings, renumbering the group so the positions stay contiguous. A position outside the group is clamped into it.

## `public void LEngineSenseDelete(string id)`

Deletes the Meaning identified by `id` with everything it owns: its subordinate Meanings, the relations originating inside that subtree, and its Example, Tag and Situation association rows. Refused while a relation from outside the subtree or a Collocation synonym still points at one of these Meanings.

## `public LCollocation LEngineCollocationCreate(LCollocation collocation)`

Creates `collocation` at the end of its Entry's Collocations and returns it with its assigned id and position.

## `public IReadOnlyList<LCollocation> LEngineCollocationRead(string ownerId, LOwner owner)`

Reads the Collocations of the Entry identified by `ownerId`, in stored order.

## `public void LEngineCollocationUpdate(LCollocation collocation)`

Rewrites the title, expression and meaning of the Collocation `collocation` identifies.

## `public void LEngineCollocationMove(string id, int position)`

Moves the Collocation identified by `id` to `position` in its Entry's card order, renumbering the set so the positions stay contiguous.

## `public void LEngineCollocationDelete(string id)`

Deletes the Collocation identified by `id`. Its synonym interlinks and its association rows go with it; the independent Examples, Tags and Situations they pointed at are left standing, and the Collocations left under the Entry are renumbered.
