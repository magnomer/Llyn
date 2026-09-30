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
## `internal CSounding(`

Takes the editor's desk and the phonology port every sound read goes through.
A refused load shows through `envoy`, with the ready notice `settings` reads.
The waiting checks and the morphology verdict come from `voice`, the editor's display voice.
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

## `public IReadOnlyList<CScheme> CSoundingSchemeRead(long transcription)`

The schemes the dropdown of one transcription row offers, each marked when another row holds it.
The mark is the engine's one-scheme rule, so a pick the engine would refuse is greyed out.
No held draft answers no schemes.

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
Every block's font goes through the one font rule `CCatalog.LCatalogFontRead` holds.

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

## `internal static IReadOnlyList<CParadigmSlot> CSoundingParadigmRead(`

Shapes each paradigm row the engine joined.
The lectern calls it too, so no driver groups slots.
The held flag is true for the editor, whose missing forms are held by the draft.

## `private static CParadigmSlot LSoundingSlotRead(LParadigmRow row, LParadigmStatus status, bool held)`

Maps the row's status plainly to the text shown and the key of its tip.
The status rule is Core's, so this map holds only the wording and the held choice.
A held draft answers the held key where the lectern answers the lost key.

## `public static CVariety CSoundingVarietyRead(string language, string variety)`

A variety of the language, with the key of its label and the key of its flag.
The label key is `Variety.` and the name, which a driver looks up and falls back to the name.
The flag key comes from the engine, so the ensign's key format has one owner.

## `internal static CAccent LSoundingAccentRead(string language, LAccentRow row)`

Maps one engine accent row into the ready row, reading no rule.
The editor's sheet and the reading view's block share it.

## `internal static IReadOnlyList<CContour> LSoundingContourRead(IReadOnlyList<LContour> syllables)`

Maps the engine's contour syllables into ready ones, reading no rule.
The editor's contour and the reading view's block share it.

## `internal static IReadOnlyList<CTranscriptionDraft> CSoundingTranscriptionRead(`

The transcriptions of a draft, shaped for the transcription rows and the lectern.

## `internal static CLecternAnchor LSoundingAnchorRead(`

Whether the headword offers anchoring for `entry`, and the anchor text of each row, keyed by its id.
The engine's entry-based anchor rule answers both, and the placements are joined by `CReflex.LReflexSeparator`.
The reading view and the editor's reflex block both read their anchors here, so the map has one owner.
No stored entry or a refusal answers no anchor.

## `internal static IReadOnlyList<CReflexDraft> CSoundingReflexRead(IReadOnlyList<LReflexDraft> reflexes)`

The reflexes of a draft, shaped for the reflex rows and the lectern.
The tone travels as text, so no driver reads an anatomy.
