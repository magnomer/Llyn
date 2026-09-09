# LRegisterArchive.cs

## `public sealed class LRegisterArchive`

Persists Registers — independent data no Entry, Meaning, or Collocation owns.
A Register is created once with an id and is then *referenced* by any number of Meanings and Collocations.
Each association carries the position the Register takes for that referrer alone.
Attaching and detaching therefore only ever write association rows.
Detaching leaves the Register and its other references untouched.

Two kinds of row share the table.
A row a language pack ships carries `builtin = 1` and the language it came from.
Its id is fixed, so seeding the same pack twice adds nothing.
A row the user wrote carries `builtin = 0` and no language, and its id is opaque.
Only a written row may be renamed or deleted, which is why those statements name `builtin = 0`.

A referrer's order is a unique index.
So attaching and detaching renumber that referrer's whole set through `LDatabaseOrder`.
A caller names the index it wants and never has to find a free position.

## `public LRegisterArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LRegister LRegisterCreate(LRegister register)`

Inserts `register` with a fresh id when it carries none, and returns the stored Register.
The new Register is referenced by nothing until it is attached to a referrer.

## `public void LRegisterDefaultCreate(IReadOnlyList<LRegister> registers)`

Writes the rows a language pack ships, skipping every id already present.
Seeding is therefore safe to run on every read of a language's shelf.
A default the user later renamed is never overwritten, because a built-in id is never rewritten here.

## `public LRegister? LRegisterRead(string id)`

Reads the Register identified by `id`, or `null` when no such Register exists.

## `public IReadOnlyList<LRegister> LRegisterRead()`

Reads the whole shelf, the rows a language pack ships first and the written rows after.

## `public IReadOnlyList<LRegister> LRegisterMeaningRead(string meaningId)`

Reads the Registers a Meaning is marked with, in the order that Meaning gives them.

## `public IReadOnlyList<LRegister> LRegisterCollocationRead(string collocationId)`

Reads the Registers a Collocation is marked with, in the order that Collocation gives them.

## `public void LRegisterNameUpdate(string registerId, LStateValue name)`

Rewrites the visible name of a written Register and never its id.
A row a language pack ships is left as it stands, because the pack owns its wording.

## `public int LRegisterReferenceRead(string id)`

Counts how many Meanings and Collocations reference one Register.

## `public IReadOnlyDictionary<string, int> LRegisterReferenceRead()`

Counts every referenced Register in one read, so a shelf never asks once per row.
A Register nothing references is absent rather than present as zero.

## `public void LRegisterDelete(string id)`

Deletes a written Register that nothing references.

## `public void LRegisterDelete(string id, bool detach)`

Deletes a written Register, first dropping every reference to it when `detach` is set.
It refuses while any reference remains, so a delete is never silently partial.
A row a language pack ships survives the statement, because the pack owns it.

## `public void LRegisterMeaningAttach(string meaningId, string registerId, int position)`

Marks a Meaning with a Register at the index the caller names.

## `public void LRegisterCollocationAttach(string collocationId, string registerId, int position)`

Marks a Collocation with a Register at the index the caller names.

## `public void LRegisterMeaningDetach(string meaningId, string registerId)`

Drops one Meaning's mark and renumbers what that Meaning still carries.

## `public void LRegisterCollocationDetach(string collocationId, string registerId)`

Drops one Collocation's mark and renumbers what that Collocation still carries.
