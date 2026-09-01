# LExampleLink.cs

## `public sealed class LExampleLink`

Owns the three association tables that reach an Example — from an Entry, a Meaning, or a Collocation. It lives beside `LExampleArchive` rather than inside it because attaching is a different responsibility from storing: nothing here creates, changes, or deletes an Example, and every method writes only the rows that link one to a referrer.

Each association carries the position the Example takes *for that referrer*, so the same Example may be first under an Entry and third under a Meaning. That order is a unique index, so attaching, detaching, and moving all renumber the referrer's whole set through `LDatabaseOrder`; a caller therefore names the index it wants and never has to leave a gap or find a free position.

## `public LExampleLink(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LExample> LExampleEntryRead(string entryId)`

Reads the Examples an Entry references, in the order that Entry gives them.

## `public IReadOnlyList<LExample> LExampleSenseRead(string senseId)`

Reads the Examples a Meaning references, in the order that Meaning gives them.

## `public IReadOnlyList<LExample> LExampleCollocationRead(string collocationId)`

Reads the Examples a Collocation references, in the order that Collocation gives them.

## `public void LExampleEntryAttach(string entryId, string exampleId, int position)`

References an existing Example from an Entry at `position` in that Entry's order.

## `public void LExampleSenseAttach(string senseId, string exampleId, int position)`

References an existing Example from a Meaning at `position` in that Meaning's order.

## `public void LExampleCollocationAttach(string collocationId, string exampleId, int position)`

References an existing Example from a Collocation at `position` in that Collocation's order.

## `public void LExampleEntryDetach(string entryId, string exampleId)`

Removes an Entry's reference to an Example. The Example and its other references survive.

## `public void LExampleSenseDetach(string senseId, string exampleId)`

Removes a Meaning's reference to an Example. The Example and its other references survive.

## `public void LExampleCollocationDetach(string collocationId, string exampleId)`

Removes a Collocation's reference to an Example. The Example and its other references survive.

## Inline notes

### `private void LExampleReferenceAttach(`

The three association tables differ only in their name and their referrer column, so the reference operations share one implementation each. Both identifiers are store-owned literals chosen by the methods above, never caller input, so composing them into the statement text opens no injection seam; every value still travels as a parameter.

The row is inserted beyond the end of the set and the whole set is then renumbered around it, so the requested index is honoured — and a position that is already taken is no longer a unique-index failure the caller has to work around.
