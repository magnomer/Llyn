# CSentence.cs
Hash: `b12bafb3bfc396ed`

## `public sealed class CSentence`

The sentence gates of the entry editor, one per user action on a card's sentence rows.
Each gate holds the interaction only and calls one ShellEngine member on the held draft.
The rules about the rows live in the Application clerk, so no gate trims or clamps.
The add gate alone checks its row index, since Conduct defines what that index means.
Most gates do nothing while the desk is filling a draft, since the quill reads null then.

## `internal CSentence(CDesk desk, LSentencePort sentences, LDraftPort drafts, LSettingsPort settings, CEnvoy envoy, CLedgerNoticed noticed)`

Builds the gates over the editor's desk and the sentence port.
The draft port reads the chip lines, and the settings port with the envoy reports a failed read.
It takes the atelier's repaint memory for its list reads.

## `public event Action? CSentenceReferenceChanged;`

The catalog of references changed, so the citation list of the sentences is read again.
It is raised on the driver's thread, through the marshal the editor was handed.

## `internal void LSentenceObserverAttach(Action<Action> marshal)`

Hears the reference subject on every tenure the desk starts.

## `public void CSentenceAdd(long cardId, int below)`

The gate for the add button on a sentence row.
`below` is the index of the pressed row in the card's `CCardDraftSentence` that Conduct handed out.
`CFolio` maps the sentences in the engine's order, so that index is the engine's index.
The gate checks the index against the held card's sentences, so a stale or foreign index sends nothing.
The user asks for a row below the one pressed, so the new row takes the next place.

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

## `public void CSentenceMentionAdd(long cardId, long sentenceId, string text, int start, int length, long? entryId)`

The gate for an Entry picked for a card sentence's selection, with the box's raw text and selection.
The span rule and the request are ShellEngine's, shared with the corpus transcript's gate.
The driver hands the raw pick, a null id for a fresh row.
The engine has no new Entry path for a Mention.
So a null id shows `Refusal.TargetMissing` through the envoy and links nothing.
Null no longer means silence, since a fresh pick and a silence are different user actions.

## `public void CSentenceSilenceSet(long cardId, long sentenceId, string text, int start, int length)`

The gate for the silence command on a card sentence's selection.
A silent Mention links nothing, so the selection stands for nothing.
It has its own gate, so the driver never sends the engine's id 0.
Conduct maps the silence to the engine's Entry 0 here.

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

## `public CMentionSense? CSentenceSenseRead(long cardId, long sentenceId, string text, int start, int length)`

The sense menu, ready to show, for the linked Mention under the selection.
It answers null when no linked Mention lies there, so no menu opens.
The shared read in `CMention` chooses the fallback key `Display.Unknown`, which the engine words.
A failed read shows `Mention.FindFailed` and answers null.

## `public IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> CSentenceMentionRead()`

The chip lines of every sentence row in the held draft, keyed by the row.
It answers one entry for every sentence of the held cards, and an empty list means no chips.
`CMention.LMentionLineRead` builds the entries from the draft's sentences, so no row is missing.
A failed read shows `Mention.FindFailed` once and answers an empty list for every sentence.
With no draft held it answers no entries, since there is no sentence to key.
Each unlinked chip carries its own key, so the driver passes no silent word.

## `public CSentenceFrame CSentenceFrameRead()`

The frame every sentence row of the held draft offers, in the draft's language.
It answers the word order, the particles and the dependences at once.
A failed particle or dependence read leaves that list empty, so the editor still opens.
The failure shows `Sentence.ParticleFailed` or `Sentence.DependenceFailed`.
