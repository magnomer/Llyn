# LSchemaRegister.cs

## `public static class LSchemaRegister`

Creates the independent Register and the two link tables a Meaning and a Collocation reach it by.

## `public static void LSchemaRegisterCreate(SqliteConnection connection)`

Both links carry a position, so a card holds its registers in order.
The link cascades with the card and never with the Register.
So a Register survives every detach, and the store refuses to delete one anything still points at.

A built in Register is named by its language and the key its language pack gave it.
That pair is unique.
The pack is re-read on every load of its language.
The unique pair is what turns the second read into a no-op.
It also decides how a merge treats a built in Register from another workspace.
Such a row is never inserted, because the pair already exists here.
The import must find the local row by that pair and point every link at it instead.
