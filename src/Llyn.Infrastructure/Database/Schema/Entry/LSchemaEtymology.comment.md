# LSchemaEtymology.cs

## `public static class LSchemaEtymology`

The etymology tables: what an entry says about where it came from.

## `public const long LSchemaEtymologyDeclared = 74;`

The first schema version whose workspaces carry an etymology at all.

## `public static void LSchemaEtymologyCreate(SqliteConnection connection)`

Creates the `etymology`, `etymology_mention` and `etymon` tables when they are missing.
An entry has at most one `etymology` row, which is why `entry_parent` is unique there.
Each `etymology_mention` row is one span of that prose and names the Entry the span means.
Each `etymon` row is one direct link, held in `position` order under the entry that declares it.
Every row goes when its entry goes, and a link goes when its target entry goes.
A workspace from before version 74 arrives with all three tables empty.
