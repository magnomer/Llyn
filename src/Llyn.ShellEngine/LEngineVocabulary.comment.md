# LEngineVocabulary.cs

## `public sealed partial class LEngine`

The vocabulary facade of the engine, over parts of speech, morphology, inflections and paradigms.
Every call takes the gate and hands the work to the vocabulary, inflection or paradigm clerk.
The facade stays because the shell calls the engine, and the engine alone holds the gate and the pending fetch.

## `internal LSpeechValue LEngineSpeechCreate(LSpeechValue value)`

The vocabulary clerk's create under the gate.
Every other speech, feature, morphology and sentence call is the same relay for the clerk member of the same shape.

## `private void LEngineLanguageImport()`

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

## `private IReadOnlyList<LParadigmSlot> LEngineParadigmRead(LEntry entry)`

The same read for an entry already in hand, for the fetch that runs under the gate already.

## `private void LEngineParadigmUpdate(LEntry entry)`

The clerk judges every stored form of the entry, for the fetch and the workspace update.

## `internal static bool LEngineParadigmMatch(LParadigm paradigm, string headword, string form)`

Whether `form` is a regular inflection of `headword` under the paradigm's rules, kept for the tests that ask the engine.
