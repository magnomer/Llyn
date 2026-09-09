# LExampleLink.cs

## `public sealed class LExampleLink`

Reads the quoting side of an Example: which Meanings and Collocations quote it.
A Meaning and a Collocation reach their Examples through `LSentenceArchive`, because that association carries data.
This class owns no association table of its own.
It lives beside `LExampleArchive` rather than inside it.
Reporting who quotes an Example is a different responsibility from storing one.
Nothing here creates, changes, or deletes an Example.

An Entry does not quote an Example.
It reaches one only through the cards beneath it, so there is no Entry arm to read, attach, or detach.

## `public LExampleLink(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LUsage> LExampleUsageRead(string id)`

Every card quoting one Example, itemized rather than counted.
A Meaning or Collocation row names the Entry it belongs to.
A row carries the entry id it is followed through.
It stays followable after the text it shows is edited.
A quoting card with no wording of its own falls back to the definition or expression beneath it.

## `internal static void LExampleLinkClear(SqliteConnection connection, string exampleId)`

Drops every reference to one Example from every association table.
The Meaning and Collocation sides are handed to `LSentenceArchive`, which also clears it where it stands as a revision.
It takes the caller's connection, so the clearing and the delete that follows commit together.
