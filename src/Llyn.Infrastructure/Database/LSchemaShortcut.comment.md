# LSchemaShortcut.cs

## `public static class LSchemaShortcut`

Version 32.
Retires entry_source and entry_example, the two tables that linked an Entry straight to a Source or an Example.
Both skipped the levels between, so a workspace could state a citation the chain of cards did not carry.
An Entry now reaches a Source only through the Examples its Meanings and Collocations cite.

## `public static void LSchemaShortcutRemove(SqliteConnection connection)`

Drops both shortcut tables and the indexes over them.
The record runs first, because it has to read entry_source while the table still stands.
entry_example is dropped unconditionally, because nothing ever wrote it and there is nothing to report.

## `public static void LSchemaShortcutRecord(SqliteConnection connection)`

Counts the entry_source rows naming a Source no Example under that same entry cites, and writes that count to the audit log.
Such a row is the citation that cannot be carried into the chain, so it is the loss worth stating.
The count is per entry, because a Source cited only under another entry still left this entry's bibliography.
The number is a log line rather than data, so it is read once by a person and never by the program.
A workspace that loses nothing writes nothing, because a log entry saying zero is noise.
The log stands beside the database file, which is where the migration finds the workspace root.
