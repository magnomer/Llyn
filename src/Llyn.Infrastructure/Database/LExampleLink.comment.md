# LExampleLink.cs

## `public sealed class LExampleLink`

Owns the association table that reaches an Example from an Entry.
A Meaning and a Collocation reach their Examples through `LSentenceArchive`, because that association carries data.
It lives beside `LExampleArchive` rather than inside it.
Attaching is a different responsibility from storing.
Nothing here creates, changes, or deletes an Example.
Every method writes only the rows that link one to a referrer.

The association carries the position the Example takes *for that referrer*.
So the same Example may be first under an Entry and third under a Meaning.
That order is a unique index.
So attaching, detaching, and moving all renumber the referrer's whole set through `LDatabaseOrder`.
A caller therefore names the index it wants.
It never has to leave a gap or find a free position.

## `public LExampleLink(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LExample> LExampleEntryRead(string entryId)`

Reads the Examples an Entry references, in the order that Entry gives them.

## `public void LExampleEntryAttach(string entryId, string exampleId, int position)`

References an existing Example from an Entry at `position` in that Entry's order.

## `public void LExampleEntryDetach(string entryId, string exampleId)`

Removes an Entry's reference to an Example.
The Example and its other references survive.

## Inline notes

### `private void LExampleReferenceAttach(`

The table and the referrer column are store-owned literals chosen by the methods above.
They are never caller input.
So composing them into the statement text opens no injection seam.
Every value still travels as a parameter.

The row is inserted beyond the end of the set.
The whole set is then renumbered around it, so the requested index is honoured.
A position that is already taken is no longer a unique-index failure to work around.

## `public IReadOnlyList<LUsage> LExampleUsageRead(string id)`

Every side quoting one Example, itemized rather than counted.
An Entry row names itself, and a Meaning or Collocation row names the Entry it belongs to.
A row carries the entry id it is followed through, so it stays followable after the text it shows is edited.
A referring side with no wording of its own falls back to the definition or expression beneath it.

## `internal static void LExampleLinkClear(SqliteConnection connection, string exampleId)`

Drops every reference to one Example from every association table.
The Meaning and Collocation sides are handed to `LSentenceArchive`, which also clears it where it stands as a revision.
Each referrer whose set lost a row is renumbered, so no order keeps a gap.
It takes the caller's connection, so the clearing and the delete that follows commit together.

