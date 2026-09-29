# CSounding.cs

## `public sealed class CSounding`

The sound sheet of the entry the editor holds: its rime-book groups, its script rows and its paradigm slots.
It also holds the maps from an entry's sound rows to the shapes the drivers show.
Every read and gate works on the entry the desk has stored, so no driver passes an entry.
A fresh draft has no stored entry, so every read answers empty and every gate does nothing.
Every read swallows a refusal and answers empty, since a box that cannot fetch still has to draw.
Every gate announces the change, or asks the envoy to show the refusal under its own notice key.


## `internal void LSoundingObserverAttach(Action<Action> marshal)`

Hears the tenure's fanqie subject and raises `CSoundingChanged` on the driver's thread.
A fanqie changed elsewhere refreshes the editor as a fanqie set here does.
## `internal CSounding(CDesk desk, LPhonologyPort phonology, LDraftPort drafts, LSettingsPort settings, CEnvoy envoy)`

Takes the editor's desk, the phonology port every sound read goes through, and the draft port the anchors ask.
A refused load shows through `envoy`, with the ready notice `settings` reads.
Only the editor builds one, so the constructor is internal.

## `public event Action? CSoundingChanged;`

A gate landed, so the boxes drawn from the sheet are read again.

## `private long? LSoundingEntry`

The entry the desk has stored, or null for a fresh draft.

## `private string LSoundingLanguage`

The language of the draft on the desk, which picks the tones an anchor scan compares.

## `public IReadOnlyList<CFanqieGroup> CSoundingFanqieRead()`

The entry's rime-book groups, fetched first when missing.

## `public string CSoundingReadingRead(string headword)`

The headword's representative reading, formed by the engine from the same groups the box draws.

## `public bool CSoundingAnchorCheck(string headword)`

Whether the entry's rime-book rows let a reflex anchor at all.

## `public string CSoundingAnchorFormat(IReadOnlyList<long> anchors, string headword)`

The anchors of one reflex written against the entry's rime-book rows.
The placements are joined by `CReflex.LReflexSeparator`, so both panes print the same label.

## `public IReadOnlyList<CScheme> CSoundingSchemeRead(long transcription)`

The schemes the dropdown of one transcription row offers, each marked when another row holds it.
The mark is the engine's one-scheme rule, so a pick the engine would refuse is greyed out.
No held draft answers no schemes.

## `public IReadOnlyList<CAnchorRow> CSoundingAnchorScan(IReadOnlyList<long> anchors, string reflex, string tone)`

The rows one reflex may anchor to, shaped for the anchor menu.
The engine compares tones in the draft's language, which the desk supplies.

## `public void CSoundingFanqieResolve()`

The user asked to fetch the rime-book rows again.

## `public void CSoundingFanqieSet(long fanqieId, int rank)`

The user ranked one rime-book row among the entry's representative readings.
Rank zero unmarks the row, and the last rank appends it after the rows already marked.

## `public IReadOnlyList<CScriptGroup> CSoundingScriptRead()`

The entry's script rows, fetched first when missing.

## `public void CSoundingScriptResolve()`

The user asked to drop the entry's script rows and fetch them again.

## `public IReadOnlyList<CParadigmSlot> CSoundingParadigmRead()`

The entry's paradigm rows, joined by the engine and shaped for the paradigm box.

## `public string CSoundingLanguageRead()`

The language the entry's paradigm is written in, which picks the paradigm box's font.

## `private IReadOnlyList<LSoundingItem> LSoundingListRead<LSoundingItem>(`

One list read for the stored entry, empty for a fresh draft or a refusal.

## `private static LSoundingAnswer LSoundingAnswerRead<LSoundingAnswer>(`

One engine answer, or the fallback when the engine refuses.

## `private void LSoundingMarkSend(Action<long> mark, string key)`

Sends one change for the stored entry, then announces it or shows the refusal under `key`.

## `internal static IReadOnlyList<CFanqieGroup> CSoundingFanqieRead(IReadOnlyList<LFanqieGroup> groups)`

The one map from the engine's fanqie groups to their shape.
The lectern calls it too, so both fanqie tables draw the same shape.

## `private static CFanqieRow LSoundingRowRead(LFanqieRow row)`

Copies every cell the fanqie line shows, derived cells included.

## `internal static IReadOnlyList<CScriptGroup> CSoundingScriptRead(IReadOnlyList<LScriptGroup> groups)`

The one map from the engine's script groups to their shape, shared with the lectern.

## `internal static IReadOnlyList<CParadigmSlot> CSoundingParadigmRead(IReadOnlyList<LParadigmRow> rows)`

Shapes each paradigm row the engine joined.
The lectern calls it too, so no driver groups slots.

## `private static CParadigmSlot LSoundingSlotRead(LParadigmRow row)`

Copies the row's part, its name, and the first slot's form and doubt.

## `internal static IReadOnlyList<CPronunciationDraft> CSoundingPronunciationRead(`

The pronunciations of a draft, shaped for the accent rows and the lectern.

## `internal static CPronunciationDraft CSoundingPronunciationRead(LPronunciationDraft spoken)`

One pronunciation, shaped for the accent rows and the respelling resolve.

## `public static CVariety CSoundingVarietyRead(string language, string variety)`

A variety of the language, with the key of its label and the key of its flag.
The label key is `Variety.` and the name, which a driver looks up and falls back to the name.
The flag key comes from the engine, so the ensign's key format has one owner.

## `internal static IReadOnlyList<CTranscriptionDraft> CSoundingTranscriptionRead(`

The transcriptions of a draft, shaped for the transcription rows and the lectern.

## `internal static IReadOnlyList<CReflexDraft> CSoundingReflexRead(IReadOnlyList<LReflexDraft> reflexes)`

The reflexes of a draft, shaped for the reflex rows and the lectern.
The tone travels as text, so no driver reads an anatomy.
