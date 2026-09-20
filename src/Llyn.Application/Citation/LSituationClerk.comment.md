# LSituationClerk.cs

## `public sealed class LSituationClerk`

The clerk over the Situations of the workspace.
A Situation is the context a Meaning or Collocation is used in.
It is independent data on the same terms as a Tag.
It is owned by nothing and referenced by any number of cards, each holding its own order.
Rewriting a Situation is a single seam, and every card referencing it sees the new wording at once.
The image and video rows a Situation shows are situation media, settled here as a card settles its own.
The page the repertoire panel reads is composed here too, so screen and print show one thing.

## `public LSituationClerk(LRig rig)`

Reads the vault and the situation, image and video ports out of `rig`.

## `public static LSituation LSituationClerkBlank`

What a draft naming no Situation is measured against.

## `public LSituation LSituationClerkCreate(LSituation situation)`

Creates `situation` and returns it with its assigned id and its media read back.
Its title is not identity: the same words saved twice are two Situations.
The row and its media are written in one session, so a failed picture leaves no half Situation.

## `public LSituation? LSituationClerkRead(long id)`

Reads the Situation for `id`, or `null` when none has that id.

## `public IReadOnlyList<LSituation> LSituationClerkRead()`

Every Situation in the workspace, for the panel that browses the shelf itself.

## `public IReadOnlyList<LSituation> LSituationClerkRead(long ownerId, LOwner owner)`

Reads the Situations the Meaning or Collocation identified by `ownerId` references, in the order that side holds them.

## `public LPortraitPage? LSituationClerkRead(long id, LPortraitLegend legend)`

The page of one Situation, or null when no Situation has that id.
The usage tally is counted from the rows referencing it, as the catalog counts it.

## `public IReadOnlyList<LCatalogSituation> LSituationClerkFind(string query, LCatalogOrder order)`

The Situations answering `query`, in `order`, as rows already carrying how many places reference them.
A Situation is matched over its title, its description and its kind.

## `public void LSituationClerkUpdate(LSituation situation)`

Rewrites the title, description and kind of the Situation `situation` identifies, and settles its media.

## `public void LSituationClerkAttach(long ownerId, long situationId, int position, LOwner owner)`

References the Situation from the Meaning or Collocation identified by `ownerId`, at `position`.
The set is renumbered around it.

## `public void LSituationClerkDetach(long ownerId, long situationId, LOwner owner)`

Removes one side's reference to a Situation.
The Situation and its other references survive.

## `public void LSituationClerkRemove(long ownerId, long situationId, LOwner owner)`

Removes one side's reference to a Situation and deletes the Situation when that was its last reference.
The engine wraps the two in one session, so the row is judged against the references as they stand.

## `public void LSituationClerkDelete(long id)`

Deletes the Situation identified by `id`.
Refused while any Meaning or Collocation still references it.

## `public void LSituationClerkDelete(long id, bool detach)`

The same delete, with `detach` dropping every reference first.

## `public static bool LSituationClerkMatch(LSituation one, LSituation other)`

Field by field, whether two Situations say the same thing.
Identity is left out, because a held Situation is named before the record it becomes exists.
The title, the description and the kind each count, so any of them typed is an edit.
The image and video lists count as well, so an added picture is an edit.
A blank row is not, because the commit drops it.

## `public static LPortraitPage LSituationPageRead(LSituation situation, int count, LPortraitLegend legend)`

The title heads the page, and the kind and usage tally are the chips.
The description is Markdown, carried as a note so it draws as blocks.
Pictures and videos share one unheaded section, as the vignette shows them without a heading.

## `private void LSituationMediaSync(long situationId, LSituation situation)`

Settles the Situation's Images and Videos against what is attached, through the same field sync a card uses.
A row with an id is that record, its location or span rewritten when it changed.
One without becomes a fresh record.
What the list no longer names is detached and keeps its record.

## `private static bool LSituationOwnerCheck(LOwner owner)`

Whether the side is a Collocation, a Meaning answering false and any other side refused.
