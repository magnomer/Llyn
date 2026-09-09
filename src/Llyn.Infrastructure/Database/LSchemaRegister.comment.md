# LSchemaRegister.cs

## `public static class LSchemaRegister`

Creates the independent Register and the two link tables a Meaning and a Collocation reach it by.

## `public static void LSchemaRegisterCreate(SqliteConnection connection)`

Both links carry a position, so a card holds its registers in order.
The link cascades with the card and never with the Register.
So a Register survives every detach, and the store refuses to delete one anything still points at.
