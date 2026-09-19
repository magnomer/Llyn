# LRegisterVault.cs

## `public interface LRegisterVault`

The persistence port for the Register rows the engine reads and writes.
It lists exactly what the engine asks of register storage, and nothing about how rows are kept.
`LRegisterArchive` in Infrastructure is its adapter over the workspace database.

## `LRegister LRegisterCreate(LRegister register);`

Inserts `register` with a fresh id when it carries none, and returns the stored Register.
The new Register is referenced by nothing until it is attached to a referrer.

## `void LRegisterDefaultCreate(IReadOnlyList<LRegister> registers);`

Writes the rows a language pack names, keyed by name.
A name already present keeps its row id and gains the built in flag.
So a name the user wrote before any pack named it becomes built in, and every mark on it stays.
Seeding is therefore safe to run on every read of a language's shelf.

## `LRegister? LRegisterRead(long id);`

Reads the Register identified by `id`, or `null` when no such Register exists.

## `IReadOnlyList<LRegister> LRegisterRead();`

Reads the whole shelf, the built in rows first and the written rows after.

## `IReadOnlyList<LRegister> LRegisterMeaningRead(long meaningId);`

Reads the Registers a Meaning is marked with, in the order that Meaning gives them.

## `IReadOnlyList<LRegister> LRegisterCollocationRead(long collocationId);`

Reads the Registers a Collocation is marked with, in the order that Collocation gives them.

## `void LRegisterNameUpdate(long registerId, LStateValue name);`

Rewrites the visible name of a written Register and never its id.
A row a language pack ships is left as it stands, because the pack owns its wording.

## `int LRegisterReferenceRead(long id);`

Counts how many Meanings and Collocations reference one Register.

## `IReadOnlyDictionary<long, int> LRegisterReferenceRead();`

Counts every referenced Register in one read, so a shelf never asks once per row.
A Register nothing references is absent rather than present as zero.

## `void LRegisterDelete(long id);`

Deletes a written Register that nothing references.

## `void LRegisterDelete(long id, bool detach);`

Deletes a written Register, first dropping every reference to it when `detach` is set.
It refuses while any reference remains, so a delete is never silently partial.
A row a language pack ships is left alone before anything is dropped, and not merely spared the final statement.
Otherwise a delete aimed at a shipped row would strip its mark off every card and leave the row standing.

## `void LRegisterMeaningAttach(long meaningId, long registerId, int position);`

Marks a Meaning with a Register at the index the caller names.

## `void LRegisterCollocationAttach(long collocationId, long registerId, int position);`

Marks a Collocation with a Register at the index the caller names.

## `void LRegisterMeaningDetach(long meaningId, long registerId);`

Drops one Meaning's mark and renumbers what that Meaning still carries.

## `void LRegisterCollocationDetach(long collocationId, long registerId);`

Drops one Collocation's mark and renumbers what that Collocation still carries.

## `IReadOnlyList<LRegister> LRegisterLoad(string language);`

Reads the default registers the language pack of `language` declares, unstored and in pack order.
An unknown language or a pack without registers yields an empty list.
