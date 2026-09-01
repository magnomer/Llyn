# LEngineSituation.cs

## `public sealed partial class LEngine`

The Situation half of the engine: the context a Meaning or Collocation is used in, created, read, rewritten, referenced and let go of. A Situation is independent data on the same terms as a Tag — owned by nothing, referenced by any number of cards, each holding the order it takes — so the seams mirror the Tag ones exactly, including the two ways of parting a card from one.

Rewriting a Situation is a single seam rather than a read the caller edits and writes back: title, description and kind are one row, and every card referencing it sees the new wording at once because a reference points at the row and never at a copy of its text.

## `public LSituation LEngineSituationCreate(LSituation situation)`

Creates `situation` and returns it with its assigned id. Its title is not identity: the same words saved twice are two Situations.

## `public LSituation? LEngineSituationRead(string id)`

Reads the Situation for `id`, or `null` when none has that id.

## `public IReadOnlyList<LSituation> LEngineSituationRead(string ownerId, LOwner owner)`

Reads the Situations the Meaning or Collocation identified by `ownerId` references, in the order that side holds them.

## `public void LEngineSituationUpdate(LSituation situation)`

Rewrites the title, description and kind of the Situation `situation` identifies.

## `public void LEngineSituationAttach(string ownerId, string situationId, int position, LOwner owner)`

References the Situation identified by `situationId` from the Meaning or Collocation identified by `ownerId` at `position` in that side's order, renumbering the set around it.

## `public void LEngineSituationDetach(string ownerId, string situationId, LOwner owner)`

Removes one side's reference to a Situation. The Situation and its other references survive — the rule a card edit follows.

## `public void LEngineSituationRemove(string ownerId, string situationId, LOwner owner)`

Removes one side's reference to a Situation and deletes the Situation when that was its last reference. Detach, count and delete share one session, so the row is judged against the references as they stand at that moment and no caller can compose the steps itself.

## `public void LEngineSituationDelete(string id)`

Deletes the Situation identified by `id`. Refused while any Meaning or Collocation still references it; `LEngineSituationRemove` is the seam that deletes one as its last reference goes.
