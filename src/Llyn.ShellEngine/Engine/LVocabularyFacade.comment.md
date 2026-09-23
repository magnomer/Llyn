# LVocabularyFacade.cs

## `internal sealed class LVocabularyFacade`

The engine facade for vocabulary, inflections and paradigms.
Every call takes the gate and hands the work to the vocabulary, inflection or paradigm clerk.
The facade stays because the shell calls the engine, and the engine alone holds the gate and the pending fetch.

## `public LVocabularyFacade(LEngine engine)`

Stores the engine and its gate, which the facade uses for its vocabulary operations.

## `internal LSpeechValue LEngineSpeechCreate(LSpeechValue value)`

The vocabulary clerk's create under the gate.
Every other speech, feature, morphology and sentence call is the same relay for the clerk member of the same shape.

## `internal void LEngineLanguageImport()`

Every language pack's vocabulary written into the workspace, through the clerk.
It runs whenever the engine binds to a workspace, under the constructor or the rig apply.

## `internal IReadOnlyList<LInflection> LEngineInflectionRead(long entryId)`

The inflection clerk's read under the gate.

## `internal void LEngineInflectionSet(long entryId, IReadOnlyList<LInflection> inflections)`

Cancels the pending fetch for the entry, then the clerk makes the stored list exactly `inflections`.
A hand edit is a reason to ask the web again.

## `internal void LEngineInflectionAppend(long entryId, IReadOnlyList<LInflection> inflections)`

The clerk's append under the gate, for a source contributing forms.

## `internal void LEngineInflectionMove(long entryId, int position, int target)`

The clerk's move under the gate.

## `internal void LEngineInflectionDelete(long entryId, int position)`

Cancels the pending fetch for the entry, then the clerk deletes the form, as a set does.

## `internal IReadOnlyList<LParadigmSlot> LEngineParadigmRead(long entryId)`

The paradigm clerk's read under the gate.
The read never starts a fetch, because the caller decides when to ask the web.

## `public IReadOnlyList<LParadigmSlot> LEngineParadigmShow(long entryId)`

The clerk's display read under the gate, the regular forms dropped.

## `public bool LEngineInflectionCheck(long entryId)`

Whether an inflection fetch is pending for the entry, through the lacuna clerk.

## `public void LEngineInflectionStart(long entryId)`

Starts the inflection fetch of an entry through the lacuna clerk.

## `internal static bool LEngineParadigmMatch(LParadigm paradigm, string headword, string form)`

Whether `form` is a regular inflection of `headword` under the paradigm's rules, kept for the tests that ask the engine.
