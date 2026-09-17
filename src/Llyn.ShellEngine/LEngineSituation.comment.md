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

Creates `situation` and returns it with its assigned id and its media read back.
Its title is not identity: the same words saved twice are two Situations.
The row and its media are written in one session, so a failed picture leaves no half Situation.

## `private static LSituation? LEngineSituationResolve(LSituationArchive situations, LStateValue title)`

The one stored Situation whose title reads as `title`, or `null` when none or several do.
Both sides are folded the way `LCatalog.LCatalogTextNormalize` folds, so case and edge spaces do not make a second row.
Several rows sharing a title are a question the engine cannot answer.
A new row is made rather than one guessed.
The engine holds this rule.
The form, a commit and any other client therefore all get one row for one wording.

## `public LSituation? LEngineSituationRead(long id)`

Reads the Situation for `id`, or `null` when none has that id.

## `public IReadOnlyList<LSituation> LEngineSituationRead(long ownerId, LOwner owner)`

Reads the Situations the Meaning or Collocation identified by `ownerId` references, in the order that side holds them.

## `public void LEngineSituationUpdate(LSituation situation)`

Rewrites the title, description and kind of the Situation `situation` identifies, and settles its media.

## `private void LEngineMediaSync(long situationId, LSituation situation)`

Settles the Situation's Images and Videos against what is attached, through the same `LEngineFieldSync` a card uses.
A row with an id is that record, its location or span rewritten when it changed.
One without becomes a fresh record.
What the list no longer names is detached and keeps its record.
An Image is independent data another referrer may show.
Living beside the card sync keeps one rule for how a draft row becomes a stored one.

## `public void LEngineSituationAttach(long ownerId, long situationId, int position, LOwner owner)`

References the Situation identified by `situationId` from the side identified by `ownerId`.
That side is a Meaning or a Collocation, and the reference goes in at `position`.
The set is renumbered around it.

## `public void LEngineSituationDetach(long ownerId, long situationId, LOwner owner)`

Removes one side's reference to a Situation.
The Situation and its other references survive — the rule a card edit follows.

## `public void LEngineSituationRemove(long ownerId, long situationId, LOwner owner)`

Removes one side's reference to a Situation and deletes the Situation when that was its last reference.
Detach, count and delete share one session.
So the row is judged against the references as they stand at that moment.
No caller can compose the steps itself.

## `public void LEngineSituationDelete(long id)`

Deletes the Situation identified by `id`.
Refused while any Meaning or Collocation still references it.
`LEngineSituationRemove` is the seam that deletes one as its last reference goes.

## `public IReadOnlyList<LSituation> LEngineSituationRead()`

Every Situation in the workspace, for the panel that browses the shelf itself rather than one card's references.

## `public void LEngineSituationDelete(long id, bool detach)`

Deletes the Situation, dropping every reference to it first when the user asked for that.
Without `detach` it is the refusing delete above.
With it the detaching and the delete are one operation rather than a sequence a caller composes.

The usage seams that once stood here now live in `LEngineUsage.cs`.
An Example is browsed the same way a Situation is.
The two share one seam rather than each carrying its own.

## `public IReadOnlyList<LCatalogSituation> LEngineSituationFind(string query, LCatalogOrder order)`

The Situations answering `query`, in `order`, as rows already carrying how many places reference them.
A Situation is matched over its title, its description and its kind.

## `public IReadOnlyList<LCatalogSituation> LEngineSituationFind(LVista vista)`

The Situations the repertoire panel's vista lists, with the query and order read off the vista.
The vista's filter hides languages from the entries referencing the chosen Situation, so it is not applied here.
