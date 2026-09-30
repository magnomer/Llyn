# LLanguageClerk.cs

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

## `private LGlyph? LLanguageGlyphLoad(string language)`

The glyph section the pack of `language` declares, or none for a blank language.

## `public bool LLanguagePhonemicCheck(string language)`

Whether the pack of `language` is phonemic.

## `public IReadOnlyList<LContour> LLanguageContourRead(string language, string ipa)`

The tone contour syllables of `ipa`, or nothing when the contour would stay hidden.
A blank or non-tonal language draws no contour, even over a reading with tone marks.
A reading whose syllables carry no tone draws nothing either.

## `public const int LLanguageContourFloor = LContour.LContourFloor;`

The contour's lowest pitch level, handed up so no outer layer writes the scale again.
`LLanguageContourCeiling` hands up the highest.

## `public const int LLanguageContourCeiling = LContour.LContourCeiling;`

The contour's highest pitch level, handed up so no outer layer writes the scale again.

## `public Task<string?> LLanguageFlagRead(string language, CancellationToken cancellation)`

The flag image path of `language`, fetched when the code is remote.

## `public Task<string?> LVarietyFlagRead(string language, string variety, CancellationToken cancellation)`

The flag image path of one variety of `language`, or null when the variety declares none.

## `private async Task<string?> LLanguageFlagResolve(string? code, CancellationToken cancellation)`

A rooted code is a local file and answers itself when it exists.
Any other code is fetched through the language port.
