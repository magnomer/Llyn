# CDisplaySound.cs
Hash: `82b49068cb2be02a`

## `public sealed class CDisplaySound`

The reading view's sound area: the gates and reads of the pronunciation, playback, glyph and sound blocks.
It is split from [CDisplay](CDisplay.comment.md) by role, since the header and card half keeps its own area.
The display builds one over the sound half and the header area.
So every driver over that display hears the same events.
It holds no state of its own.
The shown draft, the fold and the latest play stay in [LDisplaySound](LDisplaySound.comment.md).
The shown language and headword come from the header's C record, so no engine record is read for them.
Every read answers the empty block while nothing is shown, so a late notice paints what a close painted.

## `private static readonly CLecternAccent _cDisplayMute`

The pronunciation block while nothing is shown or the pack refused, with no reading, row or flag.

## `private static readonly CFont _cDisplayBare`

The font with nothing set, so a block with nothing shown keeps its theme.

## `private static readonly CLecternAnchor _cDisplayUnanchored`

The anchors while nothing is shown or the engine refused, with no row anchored.

## `internal CDisplaySound(`

Only the display builds its sound area, over the ports and the envoy the panel handed down.

## `public event Action? CDisplayFoldChanged;`

Raised when the fold gate sets the fold, so every lectern over the display repaints its reflex rows.

## `internal event Action<string, long>? CDisplayRowChosen;`

Raised with the library tab and the entry a glyph cell resolved to.

## `internal event Action<string, string, string>? CDisplayDiweiChosen;`

Raised with the shown language and the kind and key of a clicked rime cell.

## `internal event Action<string, string?>? CDisplayStemChosen;`

Raised with the shown language and the key of a clicked phonetic series.
The display hands all three to the atelier's navigation, which switches the tab.

## `public bool CDisplayFoldOpened`

Whether the folded reflexes are shown, the state a driver's fold repaint reads.

## `private LEntryDraft? LDisplayShown`

The shown draft the sound half holds, passed unread to the engine.

## `private long? LDisplayEntry`

The shown entry's id, which every phonology read is made for.

## `public CLecternAccent CDisplayAccentRead()`

The shown entry's pronunciation block, ready to draw.
The engine answers the readings, the brackets and the pack's verdicts in one read.
Conduct only chooses each variety's label key through `CSounding.CSoundingVarietyRead`.
Nothing shown answers the mute block, and a refused read shows `Sound.LoadFailed` and answers it too.

## `public async Task<CLecternAccent?> CDisplayEnsignLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

The load a driver starts once an entry opened, for the flags its pronunciation block draws.
The engine picks the varieties and loads nothing for a pack that names them.
`store` is the driver's own image store, handed the rows through the one flag map.
It answers the block again once the flags are in, so the driver repaints its rows.
Another entry shown meanwhile wins, so a late load answers null and paints nothing.
A refused load answers null too, since a missing flag falls back to its name.

## `private static CLecternAccent LDisplayAccentRead(LAccentSheet sheet)`

Maps the engine's block into the Conduct record, reading no rule.

## `public CLecternPlayback CDisplayPlaybackRead()`

The play button and volume tray verdicts for the shown entry.
The engine answers both, so Conduct reads no audio field.
A refused read shows `Sound.LoadFailed` and hides both.

## `public void CDisplayPlaybackStart(double volume)`

The gate for the play button.
It plays the shown draft's own recording at the level the driver's slider shows.
The engine picks the file from the draft, so Conduct reads no field of it.

## `public void CDisplayPlaybackStart(string? audio, double volume)`

The gate for an accent row's play command.
It plays that row's recording at the slider's level.

## `private void LDisplayPlaybackStart(Func<int> play)`

Runs one play and keeps its ticket in the sound half, so a later stop reaches this play alone.
A refused play shows `Sound.PlayFailed` and keeps the old ticket.

## `public void CDisplayPlaybackCancel()`

The gate for a view going away.
It stops this view's own play.
A sound another view started plays on.

## `public CLecternGlyph CDisplayGlyphRead()`

The shown entry's glyph row, ready to draw.
The heading key is chosen by `CScheme.CSchemeKeyRead`, and the cells map the engine's division by name.
A language without a glyph section, or a refused read, answers the hidden row.
A refused read also shows `Sound.LoadFailed`.

## `public bool CDisplayGlyphOpen(string character, string language)`

The gate for a glyph cell.
It opens the entry the character stands for in its language.
The engine finds or makes that entry, and it is raised for the navigation to open in the library tab.
A refused resolve shows `Glyph.OpenFailed` through the catalog's one failure owner and opens nothing.

## `public IReadOnlyList<CTranscriptionDraft> CDisplayTranscriptionRead()`

The shown entry's transcription rows, ready to draw, without the glyph row.
The engine filters them, and a refused read shows `Sound.LoadFailed` and answers none.

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

The shown entry's rime-book block, ready to draw, with its reading, anchors and font.
The anchors are read again, since new fanqie rows can change which reflex rows anchor.

## `public void CDisplayFanqieSet(long fanqieId, int rank, bool raise)`

The gate for a representative pick.
It hands the held `rank` and the raise flag for the shown entry.
The fanqie clerk resolves the new rank, as behind the editor's gate.
A refusal shows `Display.FanqieRepresentativeFailed`, as the editor's gate does.

## `public bool CDisplayDiweiOpen(bool initial, string key)`

The gate for a rime-cell click.
It raises the cell in the shown entry's language for the navigation.
The engine names the cell kind from the initial flag.
Nothing shown, or a blank key, opens nothing.

## `public bool CDisplayStemOpen(string? key)`

The gate for a phonetic-series click.
It raises the series in the shown entry's language for the navigation.
Nothing shown opens nothing.

## `public CLecternScript CDisplayScriptRead()`

The shown entry's script block, ready to draw.

## `public CLecternParadigm CDisplayParadigmRead()`

The shown entry's paradigm block, ready to draw.
Its font follows the paradigm's own language, which the engine resolves.
The slots carry their status and tip key, so the driver reads neither verdict.
The inflection fetch is started when the entry opens, never by this read.

## `private IReadOnlyList<CReflex> LDisplayReflexScan()`

The written reflex rows of the shown entry through the shared scan, none when the scan is refused.

## `private CLecternAnchor LDisplayAnchorRead(IReadOnlyList<CReflex> rows)`

Whether the shown headword offers anchoring, and each row's anchor text.
The shared anchor map `CSounding.LSoundingAnchorRead` answers both, the one the editor's block uses.

## `public CFont CDisplayFontRead(CFontRole role)`

The font of `role` in the shown language, through the catalog's one font rule.
The reading view paints its headword and its example cards from it, so no driver hands a language back.

## `private static LDisplayAnswer LDisplayAnswerRead<LDisplayAnswer>(Func<LDisplayAnswer> read, LDisplayAnswer fallback)`

Runs `read`, answering `fallback` for a refusal.
