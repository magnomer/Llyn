# LSentenceArchive.cs

## `public sealed class LSentenceArchive`

Owns the association that reaches an Example from a Meaning or a Collocation.
It stands apart from `LExampleLink` because this association is the one that carries data.
An owner's hold on an Example states the frame it reads the Example under.
That fact does not belong on the Example, which is shared and owned by nothing.
Nothing here creates, changes, or deletes an Example.

Each row carries the position the Example takes for that owner, and that order is a unique index.
So every write renumbers the owner's whole set through `LDatabaseOrder`.
A caller names the index it wants and never has to find a free position.

The two owners differ only in the table their rows live in and the column naming the owner.
So each public method names its owner and one private implementation does the work.
No caller ever passes a table name.

## `public LSentenceArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LSentence> LSentenceMeaningRead(string meaningId)`

Reads what a Meaning holds over its Examples, in the order that Meaning gives them.
Each row carries the frame the Meaning reads it under, and the Example itself when the row cites one.
The Example is joined loosely, because a row may state a frame and cite no Example.

## `public IReadOnlyList<LSentence> LSentenceCollocationRead(string collocationId)`

Reads what a Collocation holds over its Examples, on the same terms as a Meaning.

## `public void LSentenceMeaningSave(string meaningId, IReadOnlyList<LSentence> sentences)`

Writes a Meaning's whole set at once, in the order given.
What the Meaning held before is dropped, so the list handed in is the list that stands.
A row naming no id is given one, since a row is only identity once it is stored.

## `public void LSentenceCollocationSave(string collocationId, IReadOnlyList<LSentence> sentences)`

Writes a Collocation's whole set at once, on the same terms as a Meaning.

## `public void LSentenceMeaningAttach(string meaningId, string exampleId, int position)`

References an existing Example from a Meaning at `position` in that Meaning's order.
The row is opened with no frame, because attaching states none.

## `public void LSentenceCollocationAttach(string collocationId, string exampleId, int position)`

References an existing Example from a Collocation at `position` in that Collocation's order.

## `public IReadOnlyList<string> LSentenceParticleRead(string language)`

Reads every marker already saved under an Entry written in `language`, without duplicates.
Nothing ships a marker, so this is the only list the shell has to offer, and a store holding none returns none.

## `public IReadOnlyList<string> LSentenceDependenceRead(string language)`

Reads every role already saved under an Entry written in `language`, on the same terms as a marker.

## `public void LSentenceMeaningDetach(string meaningId, string exampleId)`

Removes a Meaning's hold on an Example.
The Example and every other owner's hold on it survive.

## `public void LSentenceCollocationDetach(string collocationId, string exampleId)`

Removes a Collocation's hold on an Example, on the same terms as a Meaning.

## Inline notes

### `internal static void LSentenceExampleClear(SqliteConnection connection, string exampleId)`

Drops every owner's hold on one Example before that Example is deleted.
Both tables are swept, because a Meaning and a Collocation may each cite it.
It takes the caller's connection, so the clearing and the delete that follows commit together.

### `private IReadOnlyList<string> LSentenceFrameRead(string column, string language)`

Both owner tables are read at once, because a marker written on a Collocation is a marker the language uses.
The language is the owning Entry's, not the cited Example's.
A frame may stand with no Example at all, and reading the language off the Example would hide exactly those rows from the field that has to offer them.
Only rows stating a value are offered, so an unreadable or an unwritten field adds nothing to the list.
The column name is a store-owned literal named by the two methods above and never caller input.

### `private static IReadOnlyList<LSentence> LSentenceOwnerRead(`

The table and the owner column are store-owned literals named by the methods above.
They are never caller input, so composing them into the statement text opens no injection seam.
Every value still travels as a parameter.

### `private static void LSentenceOwnerClear(`

Empties one owner's set, which is what a whole-set write starts from.
