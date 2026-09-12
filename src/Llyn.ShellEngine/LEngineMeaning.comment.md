# LEngineMeaning.cs

## `public sealed partial class LEngine`

The Meaning and Collocation halves of the engine.
They are the two card kinds an Entry is made of.
They are reached one row at a time rather than through the whole-form save.
The form writes and rewrites a whole draft, which is the only path the input panel needs.
These seams are the model's own: create one Meaning, move it among its siblings, delete it.
They exist whether or not a control does.

Both are ordered within their Entry, so both carry a move, and both delete downwards.
A Meaning takes its subordinate Meanings and its association rows with it.
A Collocation takes its synonym interlinks and its association rows.
The independent Examples, Tags and Situations either referenced are left standing.
Only the links go.

A Meaning is refused deletion while a link still points at it.
That link is a relation from outside the subtree, or any Collocation synonym.
A link is a statement about a Meaning that exists.
A cascade must not decide on its own to unmake someone else's statement.

## `public LMeaning LEngineMeaningCreate(LMeaning meaning)`

Creates `meaning` at the end of its Entry's Meanings.
When it names a parent, it goes at the end of that parent's subordinate Meanings.
Returns it with its assigned id and position.

## `public LMeaning? LEngineMeaningRead(long id)`

Reads the Meaning for `id`, or `null` when none has that id.

## `public IReadOnlyList<LMeaning> LEngineMeaningRead(long ownerId, LOwner owner)`

Reads the Meanings of the Entry identified by `ownerId`, in stored order.

## `public void LEngineMeaningUpdate(LMeaning meaning)`

Rewrites the fields of the Meaning `meaning` identifies.
Its place among its siblings is not touched here — `LEngineMeaningMove` owns the order.

## `public void LEngineMeaningMove(long id, int position)`

Moves the Meaning identified by `id` to `position` among its siblings, renumbering the group so the positions stay contiguous.
A position outside the group is clamped into it.

## `public void LEngineMeaningDelete(long id)`

Deletes the Meaning identified by `id` with everything it owns.
That is its subordinate Meanings and the relations originating inside that subtree.
It is also its Example, Tag and Situation association rows.
Refused while a relation from outside the subtree or a Collocation synonym still points at one of these Meanings.

## `public LCollocation LEngineCollocationCreate(LCollocation collocation)`

Creates `collocation` at the end of its Entry's Collocations and returns it with its assigned id and position.

## `public IReadOnlyList<LCollocation> LEngineCollocationRead(long ownerId, LOwner owner)`

Reads the Collocations of the Entry identified by `ownerId`, in stored order.

## `public void LEngineCollocationUpdate(LCollocation collocation)`

Rewrites the title, expression and meaning of the Collocation `collocation` identifies.

## `public void LEngineCollocationMove(long id, int position)`

Moves the Collocation identified by `id` to `position` in its Entry's card order.
It renumbers the set so the positions stay contiguous.

## `public void LEngineCollocationDelete(long id)`

Deletes the Collocation identified by `id`.
Its synonym interlinks and its association rows go with it.
The independent Examples, Tags and Situations they pointed at are left standing.
The Collocations left under the Entry are renumbered.
