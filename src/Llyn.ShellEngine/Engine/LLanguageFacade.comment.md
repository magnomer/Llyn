# LLanguageFacade.cs
Hash: `57a159c18c9816c7`

## `internal sealed class LLanguageFacade`

The engine facade for language packs.
Everything the shell asks about a language as such is answered here.
That is the packs on disk, a pack's typography, its flag, and its regional varieties.
Each pack is read once through the language clerk's cache, so no lookup parses the file again.
The clerk is rebuilt with the workspace, since a pack's source lists belong to the folder it was read from.
The shell never reaches into the `languages/` folder itself.
The script facades sit here too, since a script style is a fact of the pack.

## `private readonly SemaphoreSlim _lLanguageFacadeEnsign`

One flag fill runs at a time, around its missing read, its fetch and its record.
Several panels ask on the same announcement, and each fill is a set of downloads.
Without the gate each would read the same flags as missing and fetch them again.
Both fills share it, so a variety fill and a language fill never overlap either.
It is awaited, never held by a plain wait, so a fill awaiting the shell can always finish.

## `public LLanguageFacade(LEngine engine)`

Stores the engine and its gate, which the facade uses for its language-pack operations.

## `public IReadOnlyList<string> LEngineLanguageRead()`

Returns the names of the languages that have a pack on disk, for the UI to offer as choices.
The clerk scans the packs once and keeps the list, so a repeated ask does not scan again.
Every panel asks for it on each entry switch, so a fresh scan each time stalled the UI thread.

## `public string LEngineGlossRead()`

Picks the gloss language of the settings when a pack for it is loaded.
Otherwise it falls back to the first loaded language, and to empty when none is loaded.
A settings file names a language the workspace may lack, so the pick checks the list first.

## `public LFont LEngineFontRead(string language, LFontRole role)`

The typography the pack declares for one role: the headword, an example line, a gloss, or a glyph chip.
The glyph role falls back to the example typography, so a chip stays serif when the section names no font.

## `public Task<string?> LEngineFlagRead(string language, CancellationToken cancellation)`

Returns the local path to the given language's flag image, for the UI to display beside it.
It returns `null` when the pack declares no flag or the download fails.
The pack declares only an ISO country code.
The engine downloads the matching flag from the flag-icons set and caches it in the workspace.
So the UI never reaches into the `languages/` folder itself.

## `public async Task<IReadOnlyList<string>> LEngineEnsignLoad(Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)`

Fetches the flag of every language not yet asked of the cache, all at once, and records what came back.
Hands the rows newly kept to `store`, so the shell draws only those and asks nothing about the rest.
A fill that a workspace change outran records nothing and calls nothing.
The cache is read once before the fetch and kept, since a workspace change builds a new one.
Asking the staff again after the fetch would meet the new cache at a matching age.
It answers the loaded languages it read, so a language menu fills from the same load.
The fetch resumes on the caller's context, so the shell's seam runs on the shell's thread.
It waits its turn at `_lLanguageFacadeEnsign`, so a second fill reads the cache after the first recorded.
A failed download answers null inside the loader, so one missing flag never stops the fill.
Any other failure, such as a file write, is not caught here and ends the fill.

## `public async Task LEngineEnsignLoad(string language, IEnumerable<string> varieties, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)`

The same fill for the named varieties of one language, keyed `language/variety`.
It waits at the same gate as the language fill.
Failures travel as in the language fill.

## `internal IReadOnlyList<LVariety> LEngineVarietyRead(string language)`

Returns the regional varieties the language pack declares, in the pack's order.
Empty when the pack declares none, so a language without varieties shows plain rows.
It reads the pack from the engine's cache, so a menu reopened does not reparse the file.

## `internal bool LEngineFlaggedCheck(string language)`

Reports whether the pack asks the UI to label a reading's variety by flag rather than by name.
It reads the cached pack as `LEngineVarietyRead` does.

## `internal bool LEngineSpacedCheck(string language)`

Reports whether the pack spaces its words, which picks the lexical units an entry is offered.
An empty language reads as spaced, the default of every pack.

## `public bool LEngineFlaggedCheck(LEntryDraft draft)`

Reads the language from the displayed draft and treats an empty language as unflagged.

## `public LAccentSheet LEngineAccentRead(LEntryDraft draft)`

The pronunciation block of `draft` as the reading view prints it.
The pack's flag and respelling verdicts and the primary's contour come in one answer with the rows.
The language clerk resolves each row, so the reading rule has its one owner below.
Only further pronunciations that carry a reading become rows.

## `public LAccentSheet LEngineAccentRead(LEntryDraft draft, IEnumerable<LPronunciationDraft> accents)`

The pronunciation block of `draft` with `accents` as its rows.
The reading view hands the notated rows, and the editor hands every row.

## `public async Task<LAccentSheet> LEngineAccentLoad(LEntryDraft draft, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)`

Loads the flags the block of `draft` draws into `store`, then answers the block.
A pack that labels varieties by name loads nothing.

## `public async Task LEngineEnsignLoad(LAccentSheet sheet, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)`

Loads the flags `sheet` draws into `store`.
A pack that labels varieties by name loads nothing.

## `public IReadOnlyList<LContour> LEngineContourRead(string language, string ipa)`

The tone contour syllables of `ipa` in `language`, ready to draw under a reading.
The language clerk owns the rule, so the sheet and the editor share it.

## `public bool LEngineSilentCheck(string language)`

Reports whether the pack declares the language silent, so the input panel knows to hide its pronunciation rows.
It reads the cached pack as `LEngineVarietyRead` does.
A blank language has no pack and answers false, so an empty desk keeps its pronunciation rows.

## `public Task<string?> LEngineVarietyResolve(string language, string variety, CancellationToken cancellation)`

Returns the local path to the flag image of one named variety of the language.
It returns `null` when the pack lacks that variety or its flag, or the download fails.
The name is matched exactly, because it is the tag the pack's own readings carry.

## `internal LLanguage LEngineLanguageLoad(string language)`

The pack named, read through the language clerk.
Every reader of a pack's declarations in the engine goes through here, so the cache alone parses the file.
The gate is taken only to read the clerk field, which a rig apply replaces.

## `public IReadOnlyList<LScriptStyle> LEngineStyleRead(string language)`

The script styles of a language.

## `public void LEngineScriptStart(long entryId)`

Starts the fetch of every character that has no images.

## `public IReadOnlyList<LTranscriptionDraft> LEngineTranscriptionRead(LEntryDraft draft)`

The filled transcription rows a reading view lists, without the glyph row, through the language clerk.
The gate is taken only to read the clerk field, which a rig apply replaces.

## `public LGlyphBlock LEngineGlyphRead(LEntryDraft draft)`

The draft's transcription rows split by its glyph section, as an editor lists them, through the language clerk.
The gate is taken only to read the clerk field, as for the transcription rows.

## `public LGlyph? LEngineGlyphRead(string language)`

The glyph section the pack of `language` declares, or null when the language shows no glyph row.
The language clerk loads it, and the gate is taken only to read the clerk field.

## `public IReadOnlyList<LGlyphCell> LEngineGlyphDivide(LEntryDraft draft)`

The cells of the draft's glyph row, through the language clerk.
The gate is taken only to read the clerk field, as for the glyph block.

## `public void LEngineSoundStart(long entryId)`

Starts every background fetch a reading view of the entry shows, in one fixed order.
The reflexes go first, then the inflections, the script images and the rime-book rows.
Each start skips what is stored or pending, so opening an entry again starts nothing twice.
The inflection start itself holds back while the morphology setting is off.

## `public bool LEngineStyleCheck(string language)`

Whether the language pack names any script style at all.

## `public void LEngineScriptRebuild(long entryId)`

Drops the stored images of the entry's characters and fetches them again.

## `public IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId)`

The images grouped by style.

## `public IReadOnlyList<LScriptGroup> LEngineScriptRead(long entryId)`

The images grouped by style, after starting the fetch of every character still missing.

## `public bool LEngineScriptCheck(long entryId)`

Whether a fetch is pending for any character of the entry.

## `private LEngineStaff LLanguageFacadeStaff`

The engine's held staff, read afresh at each use.
