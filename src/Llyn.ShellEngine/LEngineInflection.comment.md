# LEngineInflection.cs

## `public sealed partial class LEngine`

The inflection half of the engine.
It covers the inflected forms an Entry owns.
Each carries the ordered grammatical features that say what form it is.
Unlike a Tag or an Example, an inflection is not independent data.
It belongs to its Entry and is identified by its place in that Entry's list.
It goes when the Entry goes.
That is why these seams are keyed by the Entry and a position.
They are not keyed by an id of their own.

Writing the whole list and appending to it are separate seams because they answer different questions.
`LEngineInflectionSet` makes the stored list the list it is handed.
That is what an editor showing every form does.
`LEngineInflectionAppend` adds to the end without reading what is there.
That is what a source contributing forms does.
Neither is the other with an argument.

The feature ids an inflection carries are resolved for display through the morphology vocabulary.
They are not stored as words.
A form knows it is plural.
What "plural" is called in a language is a fact of that language's pack.

## `public IReadOnlyList<LInflection> LEngineInflectionRead(string entryId)`

Reads the inflected forms of the Entry identified by `entryId`, in stored order, each with its features in theirs.

## `public void LEngineInflectionSet(string entryId, IReadOnlyList<LInflection> inflections)`

Makes the stored inflections of the Entry identified by `entryId` exactly `inflections`.
They are stored in the order given, and an empty list clears them.

## `public void LEngineInflectionAppend(string entryId, IReadOnlyList<LInflection> inflections)`

Appends `inflections` to the end of the Entry's list, leaving the forms already stored where they are.

## `public void LEngineInflectionMove(string entryId, int position, int target)`

Moves the inflection at `position` in the Entry's list to `target`, renumbering the list so the positions stay contiguous.

## `public void LEngineInflectionDelete(string entryId, int position)`

Deletes the inflection at `position` in the Entry's list with its features.
It renumbers the forms after it so the list stays contiguous.
