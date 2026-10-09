# CDisplaySound.cs
Hash: `92c7ad3fda4d1c64`

## `public sealed class CDisplaySound`

The reading view's sound area, holding the gates and reads of its glyph, reflex, rime-book, script and paradigm blocks.
It is split from [CDisplay](CDisplay.comment.md) by role, since the header keeps its own area.
The pronunciation block went to [CDisplayAccent](CDisplayAccent.comment.md), and playback to [CDisplayPlayback](CDisplayPlayback.comment.md).
The header area builds one over the rules' sound half and itself.
So every driver over that area hears the same events.
It holds no state of its own.
The shown draft and the fold stay in [LDisplaySound](LDisplaySound.comment.md).
The shown language and headword come from the header's C record, so no engine record is read for them.
Every read answers the empty block while nothing is shown, so a late notice paints what a close painted.

## `private static readonly CFont _cDisplayBare`

The font with nothing set, so a block with nothing shown keeps its theme.

## `private static readonly CLecternAnchor _cDisplayUnanchored`

The anchors while nothing is shown or the engine refused, with no row anchored.

## `internal CDisplaySound(LDisplay display, CDisplay header, LDraftPort drafts, LEntryPort entries, LGlyphPort glyphs, LFanqiePort fanqies, LDiweiPort diweis, LScriptPort scripts, LParadigmPort paradigms, LReflexPort reflexes, LSettingsPort settings, CEnvoy envoy)`

Only the header area builds its sound area, over the ports and the envoy the atelier handed down.
It takes the sound half and the atelier's repaint memory from `display`, so the width did not grow.
Ledger-backed repaint reads share failure memory, while user actions report failures separately.
`glyphs` reads the glyph row and transcriptions, and `entries` opens the entry a glyph cell names.
Each sound block reads through its own narrow port, so no block reaches a slice it never shows.
The fanqie gate answers a user act, so it shows every failure.

## `public event Action? CDisplayFoldChanged;`

Raised when the fold gate sets the fold, so every lectern over the display repaints its reflex rows.

## `internal event Action<string, long>? CDisplayRowChosen;`

Raised with the library tab and the entry a glyph cell resolved to.

## `internal event Action<string, string, string>? CDisplayDiweiChosen;`

Raised with the shown language and the kind and key of a clicked rime cell.

## `internal event Action<string, string?>? CDisplayStemChosen;`

Raised with the shown language and the key of a clicked phonetic series.
The event carries two values, with a null key allowed for navigation without a selected series.

## `public bool CDisplayFoldOpened`

Whether the folded reflexes are shown, the state a driver's fold repaint reads.

## `private LEntryDraft? LDisplayShown`

The shown draft the sound half holds, passed unread to the engine.

## `private long? LDisplayEntry`

The shown entry's id, which every phonology read is made for.

## `public CLecternGlyph CDisplayGlyphRead()`

The shown entry's glyph row, ready to draw.
The heading key is chosen by `CScheme.CSchemeKeyRead`, and the cells map the engine's division by name.
A language without a glyph section, or a refused read, answers the hidden row.
A refused read also shows `Sound.LoadFailed` once.

## `public bool CDisplayGlyphOpen(string character, string language)`

The gate for a glyph cell.
It opens the entry the character stands for in its language.
The engine finds or makes that entry, and it is raised for the navigation to open in the library tab.
A refused resolve shows `Glyph.OpenFailed` through the catalog's one failure owner and opens nothing.

## `public IReadOnlyList<CTranscriptionDraft> CDisplayTranscriptionRead()`

The shown entry's transcription rows, ready to draw, without the glyph row.
The engine filters them, and a refused read shows `Sound.LoadFailed` once and answers none.

## `public CLecternReflex CDisplayReflexRead()`

The shown entry's reflex block, ready to draw.
The rows come from the one reflex scan the editor shares, `CRespelling.LRespellingReflexScan`.

## `public CLecternReflex CDisplayReflexResonate()`

Answers the reflex notice the driver hands over.
It reloads the stored rows, then reads the block again.
The reload only reaches the reflexes, so the header and the other blocks keep the draft on screen.

## `public void CDisplayReflexToggle(bool opened)`

The gate for the fold toggle.
It sets whether folded reflexes show and raises `CDisplayFoldChanged`.
The editor's reflex block and the reading view share the one fold.

## `public CLecternFanqie CDisplayFanqieRead()`

The shown entry's rime-book block, ready to draw, with its reading and font.
A refused row read shows `Display.FanqieReadFailed` through the voice's failure event.

## `public void CDisplayFanqieSet(long fanqieId, int rank, bool raise)`

The gate for a representative pick.
It hands the held `rank` and the raise flag for the shown entry.
The fanqie clerk resolves the new rank, as behind the editor's gate.
A refusal shows `Display.FanqieRepresentativeFailed` every time, as the editor's gate does.

## `public bool CDisplayDiweiOpen(bool initial, string key)`

The gate for a rime-cell click.
It raises the cell in the shown entry's language for the navigation.
The engine names the cell kind from the initial flag.
Nothing shown, or a key the engine names no kind for, opens nothing.

## `public bool CDisplayStemOpen(string? key)`

The gate for a phonetic-series click.
It raises the series in the shown entry's language for the navigation.
Nothing shown opens nothing.

## `public CLecternScript CDisplayScriptRead()`

The shown entry's script block, ready to draw.
A refused row read shows `Display.ScriptReadFailed` through the voice's failure event.

## `public CLecternParadigm CDisplayParadigmRead()`

The shown entry's paradigm block, ready to draw.
Its font follows the paradigm's own language, which the engine resolves.
The slots carry their status and tip key, so the driver reads neither verdict.
The inflection fetch is started when the entry opens, never by this read.
A refused row read shows `Display.ParadigmReadFailed` through the voice's failure event.
The inflection box is read through the same ledger read with the same failure key.
A refused box read shows that notice and answers a null view.

## `private IReadOnlyList<CReflex> LDisplayReflexScan()`

The written reflex rows of the shown entry through the shared scan.
A refused scan shows `Display.ReflexFailed` once and answers no rows.

## `private CLecternAnchor LDisplayAnchorRead(IReadOnlyList<CReflex> rows)`

Whether the shown headword offers anchoring, and each row's anchor text.
The shared anchor map `CReflex.LReflexAnchorRead` answers both, the one the editor's block uses.
It hands the map this view's envoy, settings and repaint memory, so a refusal shows `Display.AnchorFailed` once.

## `public CFont CDisplayFontRead(CFontRole role)`

The font of `role` in the shown language, through the one font rule `CFont.CFontRead` holds.
The reading view paints its headword and its example cards from it, so no driver hands a language back.
