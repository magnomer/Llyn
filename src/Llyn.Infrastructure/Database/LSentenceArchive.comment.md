# LSentenceArchive.cs

## `public sealed class LSentenceArchive`

Owns the association that reaches an Example from a Meaning.
It stands apart from `LExampleLink` because this association is the one that carries data.
A Meaning's hold on an Example states the frame it reads the Example under and the rewrite it keeps of it.
Neither fact belongs on the Example, which is shared and owned by nothing.
Nothing here creates, changes, or deletes an Example.

Each row carries the position the Example takes for that Meaning, and that order is a unique index.
So every write renumbers the Meaning's whole set through `LDatabaseOrder`.
A caller names the index it wants and never has to find a free position.

## `public LSentenceArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LSentence> LSentenceMeaningRead(string meaningId)`

Reads what a Meaning holds over its Examples, in the order that Meaning gives them.
Each row carries the Example itself and the rewrite when one stands.

## `public void LSentenceMeaningSave(string meaningId, IReadOnlyList<LSentence> sentences)`

Writes a Meaning's whole set at once, in the order given.
What the Meaning held before is dropped, so the list handed in is the list that stands.
A row naming no id is given one, since a row is only identity once it is stored.

## `public void LSentenceMeaningAttach(string meaningId, string exampleId, int position)`

References an existing Example from a Meaning at `position` in that Meaning's order.
The row is opened with no frame and no revision, because attaching states neither.

## `public void LSentenceMeaningDetach(string meaningId, string exampleId)`

Removes a Meaning's hold on an Example.
The Example and every other Meaning's hold on it survive.

## Inline notes

### `internal static void LSentenceExampleClear(SqliteConnection connection, string exampleId)`

Drops every Meaning's hold on one Example before that Example is deleted.
A row that only *cited* it as its revision is kept and loses its revision instead.
The sentence it states is untouched, so clearing a rewrite never clears the sentence.
It takes the caller's connection, so the clearing and the delete that follows commit together.

### `internal static void LSentenceMeaningClear(SqliteConnection connection, string meaningId)`

Empties one Meaning's set, which is what a whole-set write starts from.
