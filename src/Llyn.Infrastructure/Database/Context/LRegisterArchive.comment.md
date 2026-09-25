# LRegisterArchive.cs

## `public sealed class LRegisterArchive`

Persists Registers — independent data no Entry, Meaning, or Collocation owns.
A Register is created once with an id and is then *referenced* by any number of Meanings and Collocations.
Each association carries the position the Register takes for that referrer alone.
Attaching and detaching therefore only ever write association rows.
Detaching leaves the Register and its other references untouched.

The name is unique, so one name is one row whoever wrote it.
A row a language pack names carries the built in flag.
A row the user wrote carries no flag, and its id is opaque.
Only a written row may be renamed, which is why that statement names `builtin = 0`.

A referrer's order is a unique index.
So attaching and detaching renumber that referrer's whole set through `LDatabaseOrder`.
A caller names the index it wants and never has to find a free position.

## `public LRegisterArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LRegister LRegisterCreate(LRegister register)`

Inserts `register` with a fresh id when it carries none, and returns the stored Register.
The new Register is referenced by nothing until it is attached to a referrer.

## `public void LRegisterDefaultCreate(IReadOnlyList<LRegister> registers)`

Writes the rows a language pack names, keyed by name.
A name already present keeps its row id and gains the built in flag.
So a name the user wrote before any pack named it becomes built in, and every mark on it stays.
Seeding is therefore safe to run on every read of a language's shelf.

## `public IReadOnlyList<LRegister> LRegisterLoad(string language)`

The default registers the language pack declares, read through `LRegisterLoader`.
They are unstored until `LRegisterDefaultCreate` writes them.

## `public LRegister? LRegisterRead(long id)`

Reads the Register identified by `id`, or `null` when no such Register exists.

## `public IReadOnlyList<LRegister> LRegisterRead()`

Reads the whole shelf, the built in rows first and the written rows after.

## `public IReadOnlyList<LRegister> LRegisterMeaningRead(long meaningId)`

Reads the Registers a Meaning is marked with, in the order that Meaning gives them.

## `public IReadOnlyList<LRegister> LRegisterCollocationRead(long collocationId)`

Reads the Registers a Collocation is marked with, in the order that Collocation gives them.

## `public void LRegisterNameUpdate(long registerId, LStateValue name)`

Rewrites the visible name of a written Register and never its id.
A row a language pack ships is left as it stands, because the pack owns its wording.

## `public IReadOnlyDictionary<string, int> LRegisterReferenceRead()`

Counts every referenced Register in one read, so a shelf never asks once per row.
A Register nothing references is absent rather than present as zero.

## `public void LRegisterMeaningAttach(long meaningId, long registerId, int position)`

Marks a Meaning with a Register at the index the caller names.

## `public void LRegisterCollocationAttach(long collocationId, long registerId, int position)`

Marks a Collocation with a Register at the index the caller names.

## `public void LRegisterMeaningDetach(long meaningId, long registerId)`

Drops one Meaning's mark and renumbers what that Meaning still carries.

## `public void LRegisterCollocationDetach(long collocationId, long registerId)`

Drops one Collocation's mark and renumbers what that Collocation still carries.
