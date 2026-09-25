# LSituationFacade.cs

## `internal sealed class LSituationFacade`

The Situation half of the engine.
A Situation is independent data owned by nothing and referenced by any number of cards.
Every read and write goes through the situation clerk under the gate, and every change is announced here.
The vista overload stays here, because a vista is the shell's and the twin names are numbered per panel.
The situation draft starts and commits here too, through the citation clerk.

## `public LSituationFacade(LEngine engine)`

Stores the engine and its gate for situation operations.

## `public IReadOnlyList<LCatalogSituation> LEngineSituationFind(string query, LCatalogOrder order)`

The Situations answering `query`, in `order`, as rows already carrying how many places reference them.

## `public IReadOnlyList<LCatalogSituation> LEngineSituationFind(LVista vista, string unknown = "", string untitled = "")`

The Situations the repertoire panel's vista lists, with the query and order read off the vista.
Each row carries its chosen mark, true where its id is the one the vista stands on.
The twin names are read from the title.
The panel hands in the words shown for an unknown or untitled one.

## `internal LSituation? LEngineSituationRead(long id)`

Reads the Situation for `id`, or `null` when none has that id.

## `internal void LEngineSituationDelete(long id, bool detach)`

Deletes the Situation, dropping every reference to it first when the user asked for that.

## `internal LDraft LEngineSituationStart(string origin, long? situationId)`

Mints a draft id, writes the first file, and returns the held Situation.
A Situation that is gone is refused before a file is written, so no draft can point at nothing.

## `internal LSituation LEngineSituationCommit(long id)`

Turns a held Situation into a stored one, announces it and returns it.
An id from a closed workspace is refused before anything is written.
