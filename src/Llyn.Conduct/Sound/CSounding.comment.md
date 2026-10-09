# CSounding.cs
Hash: `825d46a0afbfd771`
Hash: `92f2278b2ae59a3f`

## `public sealed class CSounding`

The sound sheet of the entry the editor holds.
It covers the entry's rime-book groups, its script rows and its paradigm slots.
It also holds the maps from the fanqie, script and paradigm rows to the shapes the drivers show.
Each smaller row's map stands on its own record, such as `CVariety` or `CReflexDraft`.
Stored-entry reads and mark gates take their entry from the desk, so drivers pass no entry id.
A fresh draft has no stored rows and cannot mark them.
Rime-cell navigation instead uses the draft language and does not require a stored entry.
The open state of its boxes belongs to the user, not the entry, so `CFold` keeps it apart.
Every read answers empty on a refusal, since a box that cannot fetch still has to draw.
The refusal still shows through the envoy under the read's own notice key, so none passes unseen.
A read runs on every repaint, so it shows through the atelier's repaint memory, once until the user acts.
Every mark gate announces the change, or shows the refusal under its own notice key.

## `internal void LSoundingObserverAttach(Action<Action> marshal)`

Hears the tenure's fanqie subject and raises `CSoundingChanged` on the driver's thread.
A fanqie changed elsewhere refreshes the editor as a fanqie set here does.

## `internal CSounding(CDesk desk, LFanqiePort fanqies, LDiweiPort diweis, LScriptPort scripts, LParadigmPort paradigms, LSettingsPort settings, LDisplay display, CEnvoy envoy)`

Takes the editor's desk and the four narrow ports its rime-book, rime-cell, script and paradigm reads go through.
A refused load shows through `envoy`, with the ready notice `settings` reads.
The waiting checks and the morphology verdict come from the voice of `display`, the editor's display.
The repaint memory comes from `display` too, so the constructor takes no further parameter.
Only the editor builds one, so the constructor is internal.

## `public event Action? CSoundingChanged;`

A gate landed, so the boxes drawn from the sheet are read again.

## `internal event Action<string, string, string>? LSoundingDiweiChosen;`

A rime cell the user pressed, with the draft's language, which the editor hands to the navigation.

## `private long? LSoundingEntry`

The entry the desk has stored, or null for a fresh draft.

## `private string LSoundingLanguage`

The language of the draft on the desk, which picks the pack the blocks and the diwei open read.

## `public CSoundingFanqie CSoundingFanqieRead()`

The editor's whole rime-book block, as the reading view reads its own.
The groups are fetched first when missing, and a refusal answers them empty.
A rebuild is offered only for a stored entry whose pack has a rime book.
A refused pack check offers none.
The glyph font is the draft language's, as the editor's other glyph surfaces take it.

## `public string CSoundingReadingRead(string headword)`

The headword's representative reading, formed by the engine from the same groups the box draws.

## `public void CSoundingFanqieResolve()`

The user asked to fetch the rime-book rows again.

## `public void CSoundingFanqieSet(long fanqieId, int rank, bool raise)`

The user pressed the rank of one rime-book row among the entry's representative readings.
It hands the held rank and the raise flag down, and the fanqie clerk resolves the new rank.

## `public void CSoundingDiweiOpen(bool initial, string key)`

The user pressed a rime cell, which opens in the draft's language.
The engine names the cell kind from the initial flag, and a blank key opens nothing.

## `public CSoundingScript CSoundingScriptRead()`

The editor's whole script block, fetched first when missing.
A rebuild is offered only for a stored entry whose pack has script styles.

## `public void CSoundingScriptResolve()`

The user asked to drop the entry's script rows and fetch them again.

## `public CLecternParadigm CSoundingParadigmRead()`

The editor's whole paradigm block, in the reading view's own shape.
The rows are joined by the engine, and the waiting check and the morphology verdict come from the voice.
Both verdicts stand in each slot's ready status, so the driver never reads them.
The headword font is that of the paradigm's own language, as the reading view picks it.
Every block's font goes through the one font rule `CFont.CFontRead` holds.
The inflection box is read through the ledger under `Display.ParadigmReadFailed` and mapped as held.
A refused box read shows that notice and answers a null view.
A fresh draft has no entry, so its box is null.

## `private IReadOnlyList<LSoundingItem> LSoundingListRead<LSoundingItem>(Func<long, IReadOnlyList<LSoundingItem>> read, string key)`

One list read for the stored entry, empty for a fresh draft or a refusal.
A refusal shows `key`, which is `Display.FanqieReadFailed`, `Display.ScriptReadFailed` or `Display.ParadigmReadFailed`.
The reading view names its rows with the same keys.

## `private void LSoundingMarkSend(Action<long> mark, string key)`

Sends one change for the stored entry, then announces it or shows the refusal under `key`.
A send answers a user act, so its refusal shows every time.

## `internal static IReadOnlyList<CFanqieGroup> CSoundingFanqieRead(IReadOnlyList<LFanqieGroup> groups)`

The one map from the engine's fanqie groups to their shape.
The lectern calls it too, so both fanqie tables draw the same shape.

## `private static CFanqieRow LSoundingRowRead(LFanqieRow row)`

Copies every cell the fanqie line shows, derived cells included.

## `internal static IReadOnlyList<CScriptGroup> CSoundingScriptRead(IReadOnlyList<LScriptGroup> groups, LSettingsPort settings)`

The one map from the engine's script groups to their shape, shared with the lectern.
`settings` checks each epoch key against the interface wording.

## `private static CScriptImage LSoundingImageRead(LScriptImage image, LSettingsPort settings)`

Copies the data and caption of one script image, and turns its epoch code into a wording key.
Epoch codes come from pack data, so they form an open set and stay a string.
The key is kept only when `settings` finds a wording for it.
Otherwise the epoch is empty, so a driver never looks up a key that has no text.

## `internal static IReadOnlyList<CParadigmSlot> CSoundingParadigmRead(LParadigmPort paradigms, IReadOnlyList<LParadigmRow> rows, bool pending, bool enabled, bool held)`

Shapes each paradigm row the engine joined.
The status of each row comes from `paradigms`, so the rule keeps its one owner in the engine.
The lectern calls it too, so no driver groups slots.
The held flag is true for the editor, whose missing forms are held by the draft.

## `private static CParadigmSlot LSoundingSlotRead(LParadigmRow row, LParadigmStatus status, bool held)`

Maps the row's status to the text shown and the key of its tip.
The status rule is Core's, and the wording is `CParadigmForm.CParadigmFormResolve`'s.
This method only lays the answer into a slot.
