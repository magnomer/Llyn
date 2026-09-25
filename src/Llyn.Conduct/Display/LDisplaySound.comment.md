# LDisplaySound.cs

## `public sealed class LDisplaySound`

The reading view's sound, which holds the shown draft and plays through the engine.
It also answers which pronunciation rows show and asks the engine to divide the glyph row.
It holds the reflex fold state and makes every phonology read and fetch request for the shown entry.
A read the engine refuses answers empty, so a section draws nothing rather than failing the page.
Only the lectern's [sound](../../Llyn.UIDeportment/Display/LLecternSound.comment.md) and playback name it, so no veneer reaches Conduct.
The engine owns the player, so the view holds no player of its own.

## `public LDisplaySound(LEntryPort entries, LPhonologyPort phonology, LMediaPort media, LSettingsPort settings)`

Holds the ports the row answers, the phonology reads, the morphology setting and the playback come from.

## `public LEntryDraft? LDisplayShown => _lDisplaySoundDraft;`

The shown draft, which the lectern reads for the language and the primary pronunciation.

## `public long? LDisplayEntry => _lDisplaySoundEntry;`

The id of the shown entry, which every phonology read and fetch request is made for.

## `public bool LDisplayFoldOpened => _lDisplaySoundOpened;`

Whether the folded reflexes are shown, shared by the reading view and the editor.

## `internal void LDisplaySoundShow(long? id, LEntryDraft draft)`

Holds `draft` and its entry `id` as the entry the verdicts and reads work on.

## `internal void LDisplaySoundClear()`

Drops the shown draft, its id and the slots read, and stops playback, since what played belonged to it.

## `public bool LDisplayTonalCheck()`

Whether the shown draft's language marks tone, so the contour draws.

## `public bool LDisplayFlaggedCheck()`

Whether the shown draft's varieties draw as flags rather than labels.

## `public bool LDisplayFlaggedCheck(string language)`

Whether the shown draft is still in `language` and its varieties draw as flags.
A flag load that finishes late asks it, so another entry shown meanwhile wins.

## `public IReadOnlyList<LPronunciationDraft> LDisplayAccentRead()`

The shown draft's further pronunciations that hold notation.
A row without notation is left out, because the reading view shows only what reads.

## `public IReadOnlyList<LTranscriptionDraft> LDisplayTranscriptionRead()`

The shown draft's transcriptions that hold text, in the order the entry keeps them.
The row in the glyph scheme is left out, because the glyph row shows it as characters.

## `public LGlyph? LDisplayGlyphRead()`

The glyph section of the shown draft's language, or null when the pack declares none.

## `public IReadOnlyList<LGlyphCell> LDisplayGlyphDivide()`

The cells of the shown draft's glyph row, as the engine divides it.

## `public bool LDisplayRecordingCheck()`

Whether the shown draft owns a recording that is still on disk.

## `public bool LDisplayAudibleCheck()`

Whether the shown draft's recording exists or any notated accent names an audio file.

## `public void LDisplayRecordingPlay()`

Plays the shown draft's own recording, and does nothing when it has none.

## `public void LDisplayRecordingPlay(string? file)`

Plays an accent's recording, and the engine plays nothing for a file gone from disk.

## `public void LDisplayPlaybackStop()`

Stops what the engine plays.

## `public void LDisplayVolumeSet(double level)`

Sets the level the engine plays at.

## `public void LDisplayFoldSet(bool opened)`

Sets whether the folded reflexes are shown, as the reading view's or the editor's toggle reads.

## `public static HashSet<string> LDisplayFoldScan(IReadOnlyList<LReflexRule> rules)`

The languages `rules` fold away, compared by ordinal.

## `public void LDisplayReflexLoad()`

Reloads the shown entry after a reflex fill, keeping the held draft when the read fails.

## `public IReadOnlyList<LReflexDraft> LDisplayReflexRead()`

The shown draft's reflexes that are written, in the order the entry keeps them.

## `public void LDisplayReflexStart(long? id)`

Asks the engine to fill the entry's reflex rows.

## `public bool LDisplayReflexCheck(long? id)`

Whether a reflex fill runs for the entry, false for no entry or when the engine refuses to say.

## `public void LDisplayReflexRebuild(long? id)`

Asks the engine to drop the entry's reflex rows and fetch them again.

## `public bool LDisplayFanqieCheck(long? id)`

Whether the entry's rime-book rows are still being fetched, false for no entry or a refusal.

## `public bool LDisplayScriptCheck(long? id)`

Whether the entry's script images are still being fetched, false for no entry or a refusal.

## `public bool LDisplayParadigmCheck(long? id)`

Whether the entry's inflections are still being fetched, false for no entry or a refusal.

## `public void LDisplayFanqieStart()`

Asks the engine to fetch the shown entry's rime-book rows.

## `public IReadOnlyList<LFanqieGroup> LDisplayFanqieDivide()`

The shown entry's fanqie blocks, as the engine divides them.

## `public IReadOnlyList<LFanqieRow> LDisplayAnchorRead()`

The fanqie rows of every block in order, for the anchor text of the reflex rows.

## `public string LDisplayReadingRead()`

The headword's representative reading, formed by the engine from the blocks it divides.

## `public void LDisplayFanqieSet(long fanqieId, int rank)`

Asks the engine to make a fanqie row the shown entry's representative at `rank`.

## `public void LDisplayScriptStart()`

Asks the engine to fetch the shown entry's script images.

## `public IReadOnlyList<LScriptGroup> LDisplayScriptDivide()`

The shown entry's script rows, as the engine divides them.

## `public IReadOnlyList<LParadigmSlot> LDisplayParadigmRead()`

The shown entry's paradigm slots, held so the font verdict reads the same slots.

## `public void LDisplayInflectionStart()`

Asks the engine to fetch the shown entry's inflections, only while morphology is on.

## `public string LDisplayLanguageRead()`

The language of the first slot read, or empty when there were none.

## `public bool LDisplayMorphologyRead()`

Whether the settings turn morphology on, false when they cannot be read.

## `private IReadOnlyList<LDisplayItem> LDisplayListRead<LDisplayItem>(Func<long, IReadOnlyList<LDisplayItem>> read)`

Runs `read` for the shown entry, answering no rows for no entry or a refusal.

## `private static void LDisplayMarkSend(Action<long> mark, long? id)`

Sends `mark` for the entry, and swallows a refusal, since a fetch request answers nothing.

## `private static bool LDisplayPendingRead(Func<long, bool> check, long? id)`

Runs `check` for the entry, answering false for no entry or a refusal.
