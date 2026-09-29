# CSentence.cs

## `public sealed class CSentence`

The sentence gates of the entry editor, one per user action on a card's sentence rows.
Each gate holds the interaction only and calls one ShellEngine member on the held draft.
The rules about the rows live in the Application clerk, so no gate trims, clamps or checks.
A gate does nothing while the desk is filling a draft, since the quill reads null then.
It is the reference example of the pipeline in `docs-work/JobPrinciple.md` section 13.

## `internal CSentence(`

Builds the gates over the editor's desk and the phonology port.
The draft port reads the chip lines, and the settings port with the envoy reports a failed read.

## `private LTenure? CSentenceTenure`

The held draft, or null while the desk fills one, as the quill reads then.

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

## `public void CSentenceCitationSet(long cardId, long sentenceId, long referenceId)`

The gate for picking a stored Source from a sentence's citation dropdown.
The pick carries the Source's id, so nothing is resolved from its byline.
A typed title is `CCard.CCardCitationSet`'s action instead.

## `public void CSentenceGlossSet(long cardId, long sentenceId, long glossId, string text)`

A gloss's typed text, deferred like the sentence text.

## `public void CSentenceLanguageSet(long cardId, long sentenceId, long glossId, string language)`

The gate for a language picked in a gloss's language menu, sent at once.

## `public void CSentenceGlossAdd(long cardId, long sentenceId)`

The gate for the gloss button on a sentence row.
The engine picks the gloss language and puts the new gloss last.

## `public void CSentenceGlossRemove(long cardId, long sentenceId, long glossId)`

The gate for the cross on a gloss row.

## `public void CSentenceMentionAdd(long cardId, long sentenceId, string text, int start, int length, long entryId)`

The gate for an Entry picked for a card sentence's selection, with the box's raw text and selection.
The span rule and the request are ShellEngine's, shared with the corpus transcript's gate.
The silence command calls it too, with Entry 0, since a silent Mention is a Mention linking nothing.

## `public void CSentenceSenseSet(long cardId, long sentenceId, string text, int start, int length, long senseId)`

The gate for a sense picked in the window's Meaning menu, with the box's raw text and selection.
ShellEngine persists pending typing, finds the Mention under the selection and narrows it.

## `public void CSentenceMentionRemove(long cardId, long sentenceId, long mentionId)`

The gate for the unlink button on a chip, which names its Mention by id.

## `public void CSentenceMentionRemove(long cardId, long sentenceId, string text, int start, int length)`

The gate for the unlink command on a field, which drops the Mention the selection lies inside.

## `public bool CSentenceMentionCheck(long cardId, long sentenceId, string text, int start, int length)`

Whether the selection lies inside any Mention, the ready verdict the unlink command runs on.
It reads the held draft even while the desk fills, as the find did.

## `public bool CSentenceSenseCheck(long cardId, long sentenceId, string text, int start, int length)`

Whether the selection lies inside a Mention that links an Entry, the ready verdict of the choose command.

## `public IReadOnlyList<CMeaning>? CSentenceSenseRead(long cardId, long sentenceId, string text, int start, int length)`

The Meanings the sense menu offers, ready in reading order, for the linked Mention under the selection.
It answers null when no linked Mention lies there, so no menu opens.
It chooses the fallback key `Display.Unknown`, which the engine words, as the window's Meaning read does.
A failed read shows `Mention.FindFailed` and answers null.

## `public IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> CSentenceMentionRead()`

The chip lines of every sentence row in the held draft, keyed by the row.
A failed read shows `Mention.FindFailed` once and answers no rows, so the form keeps its lines as drawn.
Each unlinked chip carries its own key, so the driver passes no silent word.

## `public CSentenceFrame CSentenceFrameRead()`

The frame every sentence row of the held draft offers, in the draft's language.
It answers the word order, the particles and the dependences at once.
A failed particle or dependence read leaves that list empty, so the editor still opens.

## `private static IReadOnlyList<string> CSentenceListRead(Func<IReadOnlyList<string>> read)`

Runs one engine read and answers an empty list when it fails.
