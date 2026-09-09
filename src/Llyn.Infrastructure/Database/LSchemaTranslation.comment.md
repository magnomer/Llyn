# LSchemaTranslation.cs

## `public static class LSchemaTranslation`

The translations a workspace holds, in the two shapes the schema gives them.
A card links to the Entry that translates it.
An Example carries the one translation of its sentence on its own row.

## `public static void LSchemaTranslationCreate(SqliteConnection connection)`

Creates sense_translation and collocation_translation with their unique position indexes.
The tables hold nothing yet, so there is no data to move and no rebuild to run.
CREATE TABLE IF NOT EXISTS makes the step harmless on a workspace that already carries them.

## `public static void LSchemaTranslationNormalize(SqliteConnection connection)`

Retires the owned-text table an Example used to carry and puts one translation on the Example row.
An example row still holding a local column is the mark of the old shape, because LSchema cannot add one.
The rebuild is skipped when that column is gone, and the retired tables are dropped either way.
The rebuild itself belongs to `LSchemaExample`, which owns every change to that table.
