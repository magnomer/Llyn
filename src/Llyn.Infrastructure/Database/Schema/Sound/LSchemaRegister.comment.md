# LSchemaRegister.cs

## `public static class LSchemaRegister`

Creates the independent Register and the two link tables a Meaning and a Collocation reach it by.

## `public static void LSchemaRegisterCreate(SqliteConnection connection)`

Both links carry a position, so a card holds its registers in order.
The link cascades with the card and never with the Register.
So a Register survives every detach, and the store refuses to delete one anything still points at.

A Register is identified by its name, which is unique.
A pack names the Registers it offers, and never numbers them.
Two packs naming the same name name one Register, so a pack cannot claim a Register for its language.
The pack is re-read on every load of its language.
The unique name is what turns the second read into an upsert of the built in flag.
It also decides how a merge treats a Register from another workspace.
Such a row is never inserted when the name already exists here.
The import must find the local row by name and point every link at it instead.

## `private const long LSchemaRegisterShift = 1000000000;`

The distance every position is moved by while a referrer's marks are renumbered.
Moving them all out of range first is what keeps two marks from ever meeting on one position.

## `public static void LSchemaRegisterSettle(SqliteConnection connection, string schema, string other)`

Repoints, in `schema`, every mark left aimed at a Register the rebuild did not carry across.
An older workspace held one row per language for one name, and the unique name keeps only the first.
A mark on a dropped row is aimed at the surviving row of the same name, read from `other`.
A card already marked with that row keeps one mark rather than two.
Nothing is lost that the orphan sweep would otherwise have deleted.

## `private static void LSchemaLinkSettle(SqliteConnection connection, string schema, string other, string table, string column)`

Settles one link table, then renumbers every referrer's marks so they run from zero without a gap.
Replacing a doubled mark leaves a hole, and the position index expects none.
