# LSchemaFrame.cs

## `public static class LSchemaFrame`

Version 27.
A frame is the owner's, not the Example's, so an owner may state one before any sentence is written.
Both hold tables are rebuilt with a nullable `example_id` for it.
SQLite cannot drop a NOT NULL from a table that stands.

## `public static void LSchemaFrameNormalize(`

The rebuild runs with foreign keys off and back on, exactly as the version 24 rebuild does.
The two hold tables differ only in their name and their owner column, so one rebuild serves both.

## `public static void LSchemaFrameRebuild(`

The carried rows keep their own ids rather than being handed new ones.
A hold's id is what a saved draft names, so reissuing it would orphan what points at it.
Every older row is carried over whole, frame included, since each one already cites an Example.
