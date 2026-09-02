# LEngineSituation.cs

## `public sealed partial class LEngine`

The Situation half of the engine.
It is the context a Meaning or Collocation is used in.
It is created, read, rewritten, referenced and let go of.
A Situation is independent data on the same terms as a Tag.
It is owned by nothing and referenced by any number of cards, each holding its own order.
So the seams mirror the Tag ones exactly.
That includes the two ways of parting a card from one.

Rewriting a Situation is a single seam.
It is not a read the caller edits and writes back.
Title, description and kind are one row.
Every card referencing it sees the new wording at once.
A reference points at the row and never at a copy of its text.

## `public LSituation LEngineSituationCreate(LSituation situation)`

Creates `situation` and returns it with its assigned id.
Its title is not identity: the same words saved twice are two Situations.

## `public LSituation? LEngineSituationRead(string id)`

Reads the Situation for `id`, or `null` when none has that id.

## `public IReadOnlyList<LSituation> LEngineSituationRead(string ownerId, LOwner owner)`

Reads the Situations the Meaning or Collocation identified by `ownerId` references, in the order that side holds them.

## `public void LEngineSituationUpdate(LSituation situation)`

Rewrites the title, description and kind of the Situation `situation` identifies.

## `public void LEngineSituationAttach(string ownerId, string situationId, int position, LOwner owner)`

References the Situation identified by `situationId` from the side identified by `ownerId`.
That side is a Meaning or a Collocation, and the reference goes in at `position`.
The set is renumbered around it.

## `public void LEngineSituationDetach(string ownerId, string situationId, LOwner owner)`

Removes one side's reference to a Situation.
The Situation and its other references survive — the rule a card edit follows.

## `public void LEngineSituationRemove(string ownerId, string situationId, LOwner owner)`

Removes one side's reference to a Situation and deletes the Situation when that was its last reference.
Detach, count and delete share one session.
So the row is judged against the references as they stand at that moment.
No caller can compose the steps itself.

## `public void LEngineSituationDelete(string id)`

Deletes the Situation identified by `id`.
Refused while any Meaning or Collocation still references it.
`LEngineSituationRemove` is the seam that deletes one as its last reference goes.

## `public IReadOnlyList<LSituation> LEngineSituationRead()`

Every Situation in the workspace, for the panel that browses the shelf itself rather than one card's references.

## `public void LEngineSituationDelete(string id, bool detach)`

Deletes the Situation, dropping every reference to it first when the user asked for that.
Without `detach` it is the refusing delete above.
With it the detaching and the delete are one operation rather than a sequence a caller composes.

The usage seams that once stood here now live in `LEngineUsage.cs`.
An Example is browsed the same way a Situation is, so the two share one seam rather than each carrying its own.
