# LSchemaVideo.cs

## `public static class LSchemaVideo`

Creates the independent Video and the three associations that reach it.
It is shaped exactly like the Image tables, because a Video is kept exactly like an Image.

## `public static void LSchemaVideoCreate(SqliteConnection connection)`

It carries its own opaque id and the location it plays from, and nothing about it is identity.
The association cascades with the card or Situation and never with the Video.
So a Video survives every detach, and the store refuses to delete one anything still points at.
