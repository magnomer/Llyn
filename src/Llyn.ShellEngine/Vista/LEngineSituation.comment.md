# LEngineSituation.cs

## `public sealed partial class LEngine`

The Situation half of the engine.
A Situation is independent data owned by nothing and referenced by any number of cards.
Every read and write goes through the situation clerk under the gate, and every change is announced here.
The vista overload stays here, because a vista is the shell's and the twin names are numbered per panel.
The situation draft starts and commits here too, through the citation clerk.

## `internal LSituation LEngineSituationCreate(LSituation situation)`

Creates `situation` and returns it with its assigned id and its media read back.

## `internal IReadOnlyList<LSituation> LEngineSituationRead()`

Every Situation in the workspace, for the panel that browses the shelf itself.

## `public IReadOnlyList<LCatalogSituation> LEngineSituationFind(string query, LCatalogOrder order)`

The Situations answering `query`, in `order`, as rows already carrying how many places reference them.

## `public IReadOnlyList<LCatalogSituation> LEngineSituationFind(LVista vista, string unknown = "", string untitled = "")`

The Situations the repertoire panel's vista lists, with the query and order read off the vista.
Each row carries its chosen mark, true where its id is the one the vista stands on.
The twin names are read from the title.
The panel hands in the words shown for an unknown or untitled one.

## `internal LSituation? LEngineSituationRead(long id)`

Reads the Situation for `id`, or `null` when none has that id.

## `internal IReadOnlyList<LSituation> LEngineSituationRead(long ownerId, LOwner owner)`

Reads the Situations the Meaning or Collocation identified by `ownerId` references, in the order that side holds them.

## `internal void LEngineSituationUpdate(LSituation situation)`

Rewrites the title, description and kind of the Situation `situation` identifies, and settles its media.

## `internal void LEngineSituationAttach(long ownerId, long situationId, int position, LOwner owner)`

References the Situation from the Meaning or Collocation identified by `ownerId`, at `position`.

## `internal void LEngineSituationDetach(long ownerId, long situationId, LOwner owner)`

Removes one side's reference to a Situation, leaving the Situation and its other references.

## `internal void LEngineSituationRemove(long ownerId, long situationId, LOwner owner)`

Removes one side's reference to a Situation and deletes the Situation when that was its last reference.
The detach, the count, the delete and the owner's stamp share one session.

## `internal void LEngineSituationDelete(long id)`

Deletes the Situation identified by `id`, refused while any card still references it, and announces it.

## `internal void LEngineSituationDelete(long id, bool detach)`

Deletes the Situation, dropping every reference to it first when the user asked for that.

## `internal LDraft LEngineSituationStart(string origin, long? situationId)`

Mints a draft id, writes the first file, and returns the held Situation.
A Situation that is gone is refused before a file is written, so no draft can point at nothing.

## `internal LSituation LEngineSituationCommit(long id)`

Turns a held Situation into a stored one, announces it and returns it.
An id from a closed workspace is refused before anything is written.
