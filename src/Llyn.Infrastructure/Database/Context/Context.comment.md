# Context

This folder holds the stores for the data that frames a Meaning or Collocation.
It covers Registers, Situations and Tags.
They stand apart from the Entry folder because no Entry, Meaning or Collocation owns them.
Each is created once and then referenced by any number of cards through association tables.

## Shared shape

An association row carries the position the item takes for that referrer alone.
Attaching and detaching therefore only ever write association rows.
Detaching leaves the item and its other references untouched.
A referrer's order is a unique index, renumbered through `LDatabaseOrder`.
A caller names the index it wants and never has to find a free position or leave a gap.
Every store opens its session through the `database` it is bound to, and nested sessions share one transaction.

## `LRegisterArchive`

It persists Registers, which are independent data.
The name is unique, so one name is one row whoever wrote it.
A row a language pack names carries the built in flag.
A row the user wrote carries no flag, and its id is opaque.
Only a written row may be renamed, which is why that statement names `builtin = 0`.

`LRegisterCreate` inserts a Register under a fresh id.
It is referenced by nothing until attached.

`LRegisterDefaultCreate` writes the rows a language pack names, keyed by name.
A name already present keeps its row id and gains the built in flag.
So a name the user wrote before any pack named it becomes built in, and every mark on it stays.
Seeding is therefore safe to run on every read of a language's shelf.

`LRegisterLoad` gives the defaults a language pack declares, read through `LRegisterLoader`.
They are unstored until `LRegisterDefaultCreate` writes them.

`LRegisterRead` by id answers `null` when none exists.
`LRegisterRead` of the shelf lists the built in rows first and the written rows after.
`LRegisterMeaningRead` and `LRegisterCollocationRead` give the marks in the order the card gives them.

`LRegisterNameUpdate` rewrites the visible name of a written Register and never its id.
A row a language pack ships is left as it stands, because the pack owns its wording.

`LRegisterReferenceRead` counts every referenced Register in one read, so a shelf never asks once per row.
A Register nothing references is absent rather than present as zero.

The meaning and collocation attach and detach methods mark and unmark a card.
A detach renumbers what the card still carries.

## `LRegisterLoader`

It reads the Registers a language pack ships from `languages/<Name>/vocabulary.json`.
The pack is display vocabulary, so its names are what the shell shows.
A pack names its Registers and never numbers them, because the name is the identity.
A missing file, a missing `registers` section and an unknown file all mean a language shipping no Registers.
That is never a failure the caller has to handle.

`LRegisterLoaderLoad` reads them as built in rows carrying no language.
An entry that is not a non-empty string, or repeats a name already read, is skipped.
The store keys a row by name, so two packs naming the same name share one Register.
A name `LLanguageNameValidate` refuses reads nothing.

`LRegisterPackRead` reads the `registers` array of one parsed pack.
`LRegisterTextRead` gives the trimmed name of an entry, or `null` when it is not a string with text.

## `LSituationArchive`

It persists Situations, which are independent data created once with an opaque id.
Updating rewrites the visible title, description and kind, and never the id.
`LSituationDelete` refuses to run while any reference remains, unless it is asked to detach them first.

A Situation shows Images and Videos the way a Meaning does, through `situation_image` and `situation_video`.
Every read fills the two lists.
The Image and Video rows go through `LImageArchive` and `LVideoArchive`.
This store only settles which rows the Situation references and in what order.
Each association table is held as one `LDatabaseLink`, which owns the ordered attach, detach and delete.

`LSituationCreate` inserts with a fresh opaque id and returns the stored Situation.
Its Images and Videos are not written here.
The engine settles them after the row exists, as it does for a card.

`LSituationRead` by id answers `null` when none exists.
`LSituationRead` of the shelf lists every Situation in written order, only so the read is stable.
The catalog sorts them by its own order before any view shows them.
It includes one nothing references, which is reachable nowhere else.

`LSituationMeaningRead` and `LSituationCollocationRead` give a referrer's Situations in the order it gives them.

`LSituationUpdate` leaves the id and every reference untouched.
So an update never changes where the Situation appears or in what order.
It throws when no Situation carries that id.
The Images and Videos are the engine's to settle, through the attach and detach the media stores offer.

`LSituationReferenceRead` by id counts the references still pointing at the Situation.
That is the number `LSituationDelete` refuses a delete over.
A caller that has just detached one reads it to learn whether that was the last.
No store of its own has to know which association tables exist.

`LSituationReferenceRead` of the shelf counts every Situation in one statement, because a panel needs the figure on every row.
A Situation nothing references is absent rather than present as zero.

`LSituationUsageRead` answers through `LSituationUsage`, so the port has one adapter.

`LSituationDelete` drops every reference first when `detach` is asked for.
Detaching, counting and deleting share one session.
Between any two of them the answer to whether something still references the row can change.
Without `detach` the count still refuses the delete, which is the guard a card edit relies on.
The links' own sessions and the media clears nest inside this one, so the guard holds.
The media clears drop the Situation's Image and Video links before its row goes.
The cascade would do it too, but the store never leans on one it can state.

## `LSituationUsage`

It reads the referring side of a Situation, meaning which Meanings and Collocations point at it.
It lives beside `LSituationArchive`, as `LAuthorUsage` lives beside `LAuthorArchive`.
Naming the referring cards is a different responsibility from storing a Situation.
Nothing here creates, changes or deletes anything.

`LSituationUsageRead` names the referring sides rather than counting them.
A Meaning is named by its title and, standing without one, by its definition.
A Collocation is named by its title and, standing without one, by its expression.
The Entry each side belongs to is read with it, so a row is legible without a second query.
Card rows sharing a headword go by language, then entry id, then place on the Entry.
That is the default catalog order, so storage order never decides a place.

## `LTagArchive`

It persists Tags.
A Tag is a row of its own, and a card links it by id.
Two cards carrying the same word link one row rather than each writing the word.
The text is kept unique, so a word names at most one row.
The association tables hold one row per card, Tag and the position that Tag takes on that card alone.
The catalog of Tags is the tag table itself, and a row no card links stays.

A card's Tag line is written as a whole rather than edited a reference at a time.
The old links go and the new ones are numbered from zero in the order given.
Editing a card is exactly that write.
So no caller works out which Tags were added or taken away.
A card's positions are contiguous by construction rather than by repair.

`LTagMeaningSave` and `LTagCollocationSave` write the whole line, replacing whatever the card carried.
A Tag carrying an id links that row.
A Tag carrying only text links the row reading the same, resolved by trimmed text and created if absent.
Blanks and repeats are dropped.
The answer holds one row id per Tag handed in, zero for a blank.
The caller maps a draft id to its row from it.

`LTagMeaningRead` and `LTagCollocationRead` give a card's Tags in the order it gives them.
`LTagRead` answers `null` when no row carries the id.
`LTagResolve` gives the id of the row reading the text, created when none does.
`LTagCatalogRead` lists every Tag row alphabetically.

## Inline notes

### LSituationArchive.cs: private static int LSituationReferenceRead(SqliteConnection connection, long id)

This is the same count on a connection the caller already holds.
So a guard and the delete it guards run in one transaction.
Nothing can attach the row between them.

### LSituationArchive.cs: private IReadOnlyList<LSituation> LSituationReferrerRead

It reads the Situations one referrer holds through its association table, ordered by position.
The two public readers differ only in the table and column they name.
Both are store-owned literals, so composing them into the statement opens no injection seam.

### LSituationArchive.cs: private IReadOnlyList<LSituation> LSituationMediaRead(List<LSituation> situations)

It fills the media lists of a whole list in two queries, one per link table.
The rows are grouped by parent in memory.
A list read serves the catalog and the chip resolver.
Two queries per situation there would cost hundreds of round trips, so the list never asks per row.

### LSituationArchive.cs: private LSituation LSituationMediaRead(LSituation situation)

It fills the two media lists of one Situation from the Image and Video stores.
Each list keeps the order the Situation holds.
The nested sessions share the open connection, so a read stays one transaction.

### LTagArchive.cs: private IReadOnlyList<long> LTagReferrerSave

The two association tables differ only in their name and card column, so the write is one implementation.
Both identifiers are store-owned literals chosen by the callers, never caller input.
So composing them into the statement text opens no injection seam.
Every value still travels as a parameter.
A positive id that names no row is refused.
The card would otherwise be bound to a row the user never chose.
Only an id the engine minted, which is negative, is resolved by its wording.

## Stamp

Commit: `c2f335c331fe`
- `LRegisterArchive.cs` `cbafb357a0065a1b`
- `LRegisterLoader.cs` `cfb5e727228227d0`
- `LSituationArchive.cs` `df36872f476d2a41`
- `LSituationUsage.cs` `233d2ef33f66d4b3`
- `LTagArchive.cs` `6a2a80d347c380a7`
