# LSchemaReflex.cs

## `public static class LSchemaReflex`

The reflex table: the readings of an entry in the languages that borrowed its characters.

## `public static void LSchemaReflexCreate(SqliteConnection connection)`

Creates the `reflex` table when it is missing.
Each row belongs to one entry through `entry_parent` and goes with it when the entry is deleted.
The language, kind and note are plain text, unconstrained, since two rows of one language differ by kind or note.
`main` is a flag, one when the reading is the one in common use.
`respelling` is the reading recast in its own language's convention, empty when that language has no groups.
`region` is the place the reading is taken from and `remark` what the source says of it.
Both are empty when unknown.
A database from before the two columns keeps its rows, which come across with both blank.
The eight `onset_ipa` to `tone_respelling` columns hold the reading cut into its parts, in IPA and in respelling.
They are derived, never typed, and a database from before them comes across with all eight blank.
The workspace update then cuts every row again from its text under the entry's pack rules.

## `public const long LSchemaReflexNoted = 60;`

The first schema version whose reflex rows keep the note apart from the kind.

## `public static void LSchemaReflexSettle(SqliteConnection connection, string schema, long stored)`

Drops, in `schema`, every reflex row carried from a workspace older than the note.
Such rows were fetched with the pinyin or 훈 folded into the kind or the text.
No rule can unfold them.
The rows are a fetched cache, and an entry without rows is fetched again the next time it is shown.
