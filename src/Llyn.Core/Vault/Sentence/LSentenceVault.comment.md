# LSentenceVault.cs

## `public interface LSentenceVault`

The persistence port for the Sentence rows the engine reads and writes.
It lists exactly what the engine asks of sentence storage, and nothing about how rows are kept.
`LSentenceArchive` in Infrastructure is its adapter over the workspace database.

## `IReadOnlyList<LSentence> LSentenceMeaningRead(long meaningId);`

Reads what a Meaning holds over its Examples, in the order that Meaning gives them.
Each row carries the frame the Meaning reads it under, and the Example itself when the row cites one.
The Example is joined loosely, because a row may state a frame and cite no Example.

## `IReadOnlyList<LSentence> LSentenceCollocationRead(long collocationId);`

Reads what a Collocation holds over its Examples, on the same terms as a Meaning.

## `IReadOnlyList<long> LSentenceMeaningSave(long meaningId, IReadOnlyList<LSentence> sentences);`

Writes a Meaning's whole set at once, in the order given.
A row the list names by a positive id of this Meaning is rewritten in place and keeps that id.
A row named by no id, or by an id this Meaning does not hold, is inserted fresh.
A row the list stopped naming is deleted, so the list handed in is the list that stands.
The answer lists the ids in the order given, so the caller can map a draft id to its row.
A row is a thing with content of its own.
A caller holding its id therefore finds the same row after the save.

## `IReadOnlyList<long> LSentenceCollocationSave(long collocationId, IReadOnlyList<LSentence> sentences);`

Writes a Collocation's whole set at once, on the same terms as a Meaning.

## `void LSentenceMeaningAttach(long meaningId, long exampleId, int position);`

References an existing Example from a Meaning at `position` in that Meaning's order.
The row is opened with no frame, because attaching states none.

## `void LSentenceCollocationAttach(long collocationId, long exampleId, int position);`

References an existing Example from a Collocation at `position` in that Collocation's order.

## `void LSentenceMeaningDetach(long meaningId, long exampleId);`

Removes a Meaning's hold on an Example.
The Example and every other owner's hold on it survive.

## `void LSentenceCollocationDetach(long collocationId, long exampleId);`

Removes a Collocation's hold on an Example, on the same terms as a Meaning.

## `IReadOnlyList<string> LSentenceParticleRead(string language);`

Reads every marker already saved under an Entry written in `language`, without duplicates.
Nothing ships a marker, so this is the only list the shell has to offer.
A store holding none returns none.

## `IReadOnlyList<string> LSentenceDependenceRead(string language);`

Reads every role already saved under an Entry written in `language`, on the same terms as a marker.

## `LSentenceOrder LSentenceLoad(string language);`

Reads the example order the language pack of `language` declares.
A missing pack or section yields the default order.
