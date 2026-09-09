# LSchemaImage.cs

## `public static class LSchemaImage`

Creates the independent Image and the two associations that reach it.

## `public static void LSchemaImageCreate(SqliteConnection connection)`

It carries its own opaque id and the location it is loaded from, and nothing about it is identity.
The association cascades with the card and never with the Image.
So an Image survives every detach, and the store refuses to delete one anything still points at.
