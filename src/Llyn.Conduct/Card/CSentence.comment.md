# CSentence.cs

## `public sealed class CSentence`

The sentence gates of the entry editor, one per user action on a card's sentence rows.
Each gate holds the interaction only and calls one `LQuill` member on the held draft.
The rules about the rows live in the Application clerk, so no gate trims, clamps or checks.
A gate does nothing while the desk is filling a draft, since the quill reads null then.
It is the reference example of the pipeline in `docs-work/JobPrinciple.md` section 13.

## `internal CSentence(CDesk desk, LPhonologyPort phonology)`

Builds the gates over the editor's desk and the phonology port.

## `public event Action? CSentenceReferenceChanged;`

The catalog of references changed, so the citation list of the sentences is read again.
It is raised on the driver's thread, through the marshal the editor was handed.

## `internal void LSentenceObserverAttach(Action<Action> marshal)`

Hears the reference subject on every tenure the desk starts.

## `public void CSentenceAdd(long cardId, int below)`

The gate for the add button on a sentence row.
The user asks for a row below the one pressed, so the new row takes the next place.
The clerk clamps the place to the list.

## `public void CSentenceRemove(long cardId, long sentenceId)`

The gate for the erase button on a sentence row.

## `public void CSentenceTextSet(long cardId, long sentenceId, string text)`

The gate for typing in a sentence's text field.

## `public void CSentenceParticleSet(long cardId, long sentenceId, string text)`

The gate for typing in a sentence's particle field.

## `public void CSentenceDependenceSet(long cardId, long sentenceId, string text)`

The gate for typing in a sentence's dependence field.

## `public void CSentenceGlossSet(long cardId, long sentenceId, long glossId, string text)`

A gloss's typed text, deferred like the sentence text.

## `public CSentenceFrame CSentenceFrameRead()`

The frame every sentence row of the held draft offers, in the draft's language.
It answers the word order, the particles and the dependences at once.
A failed particle or dependence read leaves that list empty, so the editor still opens.

## `private static IReadOnlyList<string> CSentenceListRead(Func<IReadOnlyList<string>> read)`

Runs one engine read and answers an empty list when it fails.
