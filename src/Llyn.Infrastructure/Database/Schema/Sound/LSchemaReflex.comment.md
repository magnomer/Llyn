# LSchemaReflex.cs

## `public static class LSchemaReflex`

The reflex table: the readings of an entry in the languages that borrowed its characters.

## `public static void LSchemaReflexCreate(SqliteConnection connection)`

Creates the `reflex` table when it is missing.
Each row belongs to one entry through `entry_parent` and goes with it when the entry is deleted.
The language, kind, romanization, meaning and note are plain text.
`main` is a flag, one when the reading is the one in common use.
`owned` is one when the user supplied the meaning and a rebuild must keep it.
`respelling` is the reading recast in its own language's convention, empty when that language has no groups.
`region` is the place the reading is taken from.
The eight `onset_ipa` to `tone_respelling` columns hold the reading cut into its parts, in IPA and in respelling.
They are derived, never typed, and a database from before them comes across with all eight blank.
The workspace update then cuts every row again from its text under the entry's pack rules.

## `public const long LSchemaReflexNoted = 60;`

The first schema version whose reflex rows keep the note apart from the kind.

## `public const long LSchemaReflexParted = 72;`

The first schema version whose reflex rows keep romanization, meaning, ownership and note as separate columns.

## `public static void LSchemaReflexSettle(SqliteConnection connection, string schema, string other, long stored)`

Drops, in `schema`, every reflex row carried from a workspace older than the note.
Such rows were fetched with the pinyin or 훈 folded into the kind or the text.
No rule can unfold them.
The rows are a fetched cache, and an entry without rows is fetched again the next time it is shown.
A workspace before version 72 instead carries its old `note` into `romanization` and its old `remark` into `note`.

## `private static void LReflexPartApply(SqliteConnection connection, string schema, string other)`

Parts the old note and remark across the new romanization and note columns.
The remark is read from the attached old schema when that column exists.

## `private static bool LReflexColumnCheck(SqliteConnection connection, string schema, string column)`

Whether the named table schema has the requested reflex column.
