# LLanguageClerk.cs
Hash: `291f9818b82a4b64`

## `public sealed class LLanguageClerk`

The language packs and flags of one rig.
The listed languages are read once and kept until the rig changes.

## `public LLanguageClerk(LRig rig, LLanguageCache languages, LTrailClerk trail)`

Reads the language port out of `rig` and keeps the pack cache and the trail clerk.

## `public IReadOnlyList<string> LLanguageClerkRead()`

The language names the workspace lists, scanned once.

## `public LLanguage LLanguageClerkLoad(string language)`

The pack of `language` through the cache.

## `public bool LLanguageRespellingCheck(string language, bool respelled)`

Whether respellings show for `language`.
The setting must be on and the pack must declare respelling groups.

## `public string LLanguagePronunciationRead(LEntryDraft draft, bool respelled)`

The primary reading of `draft` as a field shows it, respelled when the setting and its pack both respell.
A draft without a reading answers empty.

## `public LAccentRow LLanguageAccentRead(LPronunciationDraft? spoken, bool respelled)`

One pronunciation as the reading view prints it, its reading resolved by the draft's own rule.
`respelled` is the pack's answer for the switch, so the caller asks it once for every row.
No pronunciation answers a blank row.

## `public static string LLanguageCandidateRead(string? phonetic, string? respelling, bool respelled)`

The text a found reading shows, by the same rule a stored pronunciation uses.
A shown respelling wins, and a blank one falls back to the phonetic text.
A missing text reads as blank.

## `public IReadOnlyList<LTranscriptionDraft> LLanguageTranscriptionRead(LEntryDraft draft)`

The filled transcription rows a reading view lists, by Core's `LGlyphOtherRead`.
The glyph section is the one the draft's pack declares, and a blank language has none.

## `public LGlyphBlock LLanguageGlyphRead(LEntryDraft draft)`

The draft's transcription rows split by its pack's glyph section, by Core's `LGlyphBlockRead`.
An editor lists both sides, blank rows included.

## `public IReadOnlyList<LGlyphCell> LLanguageGlyphDivide(LEntryDraft draft)`

The cells of the draft's glyph row, empty when its pack declares no glyph section.
The engine reaches the Core division only through this clerk.

## `public LGlyph? LLanguageGlyphLoad(string language)`

The glyph section the pack of `language` declares, or none for a null or blank language.
It is the one glyph loader, shared by the draft readers here and the engine.

## `public bool LLanguagePhonemicCheck(string language)`

Whether the pack of `language` is phonemic.

## `public IReadOnlyList<LContour> LLanguageContourRead(string language, string ipa)`

The tone contour syllables of `ipa`, or nothing when the contour would stay hidden.
A blank or non-tonal language draws no contour, even over a reading with tone marks.
A reading whose syllables carry no tone draws nothing either.

## `public static IReadOnlyList<int> LLanguageContourScale`

The contour's pitch levels from the highest down, handed up so no outer layer writes the scale again.

## `public Task<string?> LLanguageFlagRead(string language, CancellationToken cancellation)`

The flag image path of `language`, fetched when the code is remote.

## `public Task<string?> LVarietyFlagRead(string language, string variety, CancellationToken cancellation)`

The flag image path of one variety of `language`, or null when the variety declares none.

## `public string? LLanguageFlagFind(string language)`

The stored flag image path of `language`, or null when the name is blank or no file is stored.
`LLiveryFacade.LEngineLiveryRead` calls it, so a Joplin push never fetches a flag.

## `public IReadOnlyDictionary<string, string> LVarietyFlagScan(LAccentSheet sheet)`

The stored flag image path of each variety in `sheet`, keyed by variety name.
It answers none when the sheet is not flagged.
A variety the language does not declare, or whose flag file is missing, is left out.
`LLiveryFacade.LEngineLiveryRead` calls it with the page's accent sheet.

## `private string? LEnsignFind(string? code)`

The stored path for one flag code, or null for a blank code.
A code under a trail root answers itself only when the file exists.
Any other code asks `LLanguageVault.LLanguageFlagFind`, which never fetches.

## `private async Task<string?> LLanguageFlagResolve(string? code, CancellationToken cancellation)`

A rooted code is a local file and answers itself when it exists.
Any other code is fetched through the language port.
