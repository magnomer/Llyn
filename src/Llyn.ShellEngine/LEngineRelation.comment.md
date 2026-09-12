# LEngineRelation.cs

## `public sealed partial class LEngine`

The relation half of the engine.
It is the seam onto a Meaning's lexical relations and a Collocation's synonym interlinks.
It is also the seam onto the lookup that turns typed text into targets.
It sits in its own file because it answers one question the rest of the engine does not.
That question is which stored row a link points at.
It also asks how a caller finds that row before it writes.

Both kinds of link target an Entry **or** a Meaning, exclusively.
That is a rule of the model, not of the store.
`relation_entry` XOR `relation_sense` and the `CHECK` on `collocation_synonym` enforce it underneath.
Every write here refuses before it opens a session when a request is wrong.
A request is wrong when it names both targets, neither, or one not in this workspace.

**The engine never turns text into a target.**
A card field holds free text and a link needs an id.
So the two are bridged by a lookup the caller runs first.
That is `LEngineEntryFind` for entries and `LEngineMeaningFind` for meanings.
The caller passes back the id it chose.
Text that matched nothing has no id to pass.
The write is refused with `LRefusal.LRefusalTarget`.
It neither fabricates a target nor drops what was typed.
A guess written here would be a link the user never made, pointing at a word they never chose.

## `public IReadOnlyList<LMeaning> LEngineMeaningFind(string query)`

Returns the Meanings whose owning entry's headword contains `query`.
They are ordered by headword and then by each Meaning's place in its entry.
It returns every Meaning when `query` is empty or all whitespace.

This is the meaning-level twin of `LEngineEntryFind` and the second half of the resolution step.
A caller holding typed text gets back the Meanings that text could name, and picks one.
So what it hands to `LEngineRelationCreate` or `LEngineSynonymCreate` is a target the user chose.
A query matching nothing returns an empty list.
That is the answer "this text names no Meaning", not a reason to invent one.

## `public LRelation LEngineRelationCreate(LRelation relation)`

Creates `relation` at the end of its origin Meaning's relations and returns it with its assigned id and position.
The target must be one resolved row of this workspace.
Naming both an Entry and a Meaning is refused with `LRefusal.LRefusalTarget`.
So is naming neither, or naming one that is not stored.
Nothing is written in either case.

## `public IReadOnlyList<LRelation> LEngineRelationRead(string meaningId)`

Reads the relations originating from the Meaning identified by `meaningId`, in stored order.
Each carries the single target it points at.

## `public void LEngineRelationUpdate(LRelation relation)`

Updates the type and labels of the relation `relation` identifies.
Its target and its place among its siblings are not touched here.
A relation is re-pointed by deleting it and creating the one that replaces it.
So a target is only ever written alongside the check that resolved it.
`LEngineRelationMove` owns the order.

## `public void LEngineRelationMove(string id, int position)`

Moves the relation identified by `id` to `position` among the relations of its origin Meaning.
It renumbers the set so its positions stay contiguous.

## `public void LEngineRelationDelete(string id)`

Deletes the relation identified by `id`.
The Entry or Meaning it pointed at is untouched.
A link going does not take what it linked to with it.

## `public LSynonym LEngineSynonymCreate(LSynonym synonym)`

Creates `synonym` at the end of its origin Collocation's synonyms and returns it with its assigned id and position.
The target is checked on the same terms as a relation's.
It must be exactly one Entry or Meaning, and one that is stored.
Otherwise the write is refused with `LRefusal.LRefusalTarget`.

## `public IReadOnlyList<LSynonym> LEngineSynonymRead(string collocationId)`

Reads the synonym interlinks hanging from the Collocation identified by `collocationId`, in stored order.
Each carries the single target it points at.

## `public void LEngineSynonymUpdate(LSynonym synonym)`

Re-points the synonym `synonym` identifies at the target it now names.
A synonym holds nothing but its target.
So the new target is resolved here on the same terms as a create.
Both, neither, or an unstored target is refused with `LRefusal.LRefusalTarget`.

## `public void LEngineSynonymMove(string id, int position)`

Moves the synonym identified by `id` to `position` in its Collocation's order, renumbering the set so its positions stay contiguous.

## `public void LEngineSynonymDelete(string id)`

Deletes the synonym interlink identified by `id`.
The Entry or Meaning it pointed at is untouched.

## Inline notes

### `private void LEngineTargetValidate(LStateAnchor entry, LStateAnchor meaning)`

The one place a link's target is checked, for both kinds of link.
Exactly one of the two anchors points at a row.
The row it names is in this workspace.
Both conditions are the same refusal, because they are the same failure seen from two sides.
The caller did not hand over one resolved target.
It is a refusal rather than an exception because it is a request the user can correct.
The text they typed names nothing.
The shell says so instead of the save quietly writing nothing.
