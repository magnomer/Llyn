# CDisplaySound.cs
Hash: `af27f78a6c6f0e76`

## `public sealed class CDisplaySound`

The reading view's sound area provides gates and reads for glyph, reflex, rime-book, script and paradigm blocks.
The rime-book and script box openings live on [CFold](../Sound/CFold.comment.md), reached as `CDisplay.CDisplayFold`.
It is split from [CDisplay](CDisplay.comment.md) by role, since the header keeps its own area.
The pronunciation block went to [CDisplayAccent](CDisplayAccent.comment.md), and playback to [CDisplayPlayback](CDisplayPlayback.comment.md).
The header area builds one over the rules' sound half and itself.
So every driver over that area hears the same events.
It holds no state of its own.
The shown draft stays in [LDisplaySound](LDisplaySound.comment.md), and the engine stores each entry's reflex opening.
The header supplies displayed language and headword for anchors, navigation and fonts.
Glyph and transcription reads use the shown engine draft.
Block reads answer empty while nothing is shown, so late notices cannot restore closed sections.

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

## `internal event Action<string, long>? CDisplayRowChosen;`

Raised with the library tab and the entry a glyph cell resolved to.

## `internal event Action<string, string, string>? CDisplayDiweiChosen;`

Raised with the shown language and the kind and key of a clicked rime cell.

## `internal event Action<string, string?>? CDisplayStemChosen;`

Raised with the shown language and the key of a clicked phonetic series.
The event carries two values, with a null key allowed for navigation without a selected series.

## `public bool CDisplayFoldOpened`

Whether the shown entry's folded reflexes are shown, as the engine stores it for that entry.
A driver's fold repaint reads it, and nothing shown answers false.

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
The fold verdict is read from those rows by `CReflex.LReflexFoldCheck`.

## `public CLecternReflex CDisplayReflexResonate()`

Answers the reflex notice the driver hands over.
It reloads the stored rows, then reads the block again.
The reload only reaches the reflexes, so the header and the other blocks keep the draft on screen.

## `public bool CDisplayReflexToggle(bool opened)`

The gate for the reading view's "More readings" hinge.
It makes one engine call, which stores `opened` for the shown entry.
The engine then raises the fold bulletin, so `CDisplay.CDisplayFoldChanged` and the editor repaint from the store.
True means the port returned without throwing, not that a stored row changed.
Nothing shown answers false and writes nothing.
A refusal shows `Reflex.SpreadFailed` every time and answers false, so the driver puts the hinge back.

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
The slots carry their ready text and tip key, so the driver reads neither verdict.
The inflection fetch is started when the entry opens, never by this read.
A refused row read shows `Display.ParadigmReadFailed` through the voice's failure event.
The inflection box has a separate ledger-backed read using the same failure key.
A refused box read shows that notice and answers a null view.
The box is asked with `held` off, so a lost cell reads as lost.

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

## `private readonly LDisplaySound _cDisplayVoice;`

Shared sound state supplies the shown draft, entry and engine-backed checks.

## `private readonly CLedgerNoticed _cDisplayNoticed;`

Repaint failures share the display's notice memory rather than accumulating independent notices per block.

## `private readonly CDisplay _cDisplayHeader;`

The displayed header supplies language and headword for navigation, fonts and anchors.

## `private readonly LDraftPort _cDisplayDraft;`

Anchor mapping uses the draft port without exposing it to the driver.

## `private readonly LEntryPort _cDisplayEntryPort;`

Glyph navigation resolves its entry through this port.

## `private readonly LGlyphPort _cDisplayGlyphPort;`

Glyph and transcription reads share the shown draft through this port.

## `private readonly LFanqiePort _cDisplayFanqie;`

Fanqie rows, representative readings and rank changes keep one engine owner.

## `private readonly LDiweiPort _cDisplayDiwei;`

Rime navigation uses the engine's cell-kind verdict.

## `private readonly LScriptPort _cDisplayScript;`

Script blocks receive rows from their own narrow port.

## `private readonly LParadigmPort _cDisplayParadigm;`

Paradigm rows, language and inflection views keep one engine owner.

## `private readonly LReflexPort _cDisplayReflex;`

Reflex presentation and per-entry opening writes share this port.

## `private readonly LSettingsPort _cDisplaySettings;`

Typography, epoch wording and failure notices use the same settings port.

## `private readonly CEnvoy _cDisplayEnvoy;`

Failures are reported through the display's envoy rather than a driver-owned channel.
