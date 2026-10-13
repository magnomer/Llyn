# Language

The Language folder holds the language packs of one rig and what derives from a pack alone.
The cache parses each pack once.
The clerk answers readings, glyphs, contours and flags for the views.
Derived text is computed at the seams that write a row, so a view only reads what is stored.

## `LLanguageCache`

The loaded packs of one rig, read once per language and kept.
The language vault parses its file on every read, so whoever reads often must cache.
The engine and the draft clerk share one instance, so a pack is parsed once for both.
A new rig gets a new cache, since its packs may differ.
The derivations that need only a pack live here too.
Those are the respelling of a reading and the anatomy of a reflex.

### `LLanguageCacheRead`

The pack is read once and answered from memory after.
A blank name is an argument error, since no pack has no name.
The lock is the cache's own, so a caller holding another gate never waits on this one.

### `LLanguageRespellingResolve`

The pronunciation overload fills the row's respelling from its reading through the pack's respelling groups, scoped by its variety.
It runs whether or not the respelling switch is on, so both forms are always stored.
The switch then only picks which form is shown.
A blank reading, a blank language or a pack without groups leaves the respelling blank.
The form then shows the reading in its place.
Whatever the user wrote into the respelling by hand is replaced, because a stale respelling would silently mislead.
The reflex overload uses the groups of the row's own language.
It asks with no variety, so only the unscoped ones apply.
Its respelling is as bare as the reading, and the view draws the slashes of a phonemic language around it.

### `LLanguageRespellingRebuild`

Derives every pronunciation row's respelling again, for a draft whose language changed.

### `LLanguageRespellingUpdate`

Fills only the blank respellings of a draft, and recuts every reflex row.
A row the user already respelled keeps what it has, since this runs on a draft being stored, not edited.

### `LLanguageAnatomyResolve`

The row has its anatomy cut afresh under the rules of the pack of the entry's language.
A blank language, a pack without rules or a reflex language no rule names leaves the anatomy empty.
`LLanguageAnatomyScan` resolves every row in turn, order and ids kept.
`LLanguageAnatomyRebuild` resolves every reflex row of a draft under its current language, run when the language changes.

## `LLanguageClerk`

The language packs and flags of one rig.
The listed languages are read once and kept until the rig changes.

### `LLanguageRespellingCheck`

Respellings show for a language only when the setting is on and the pack declares respelling groups.

### `LLanguagePronunciationRead`

The primary reading of a draft as a field shows it, respelled when the setting and its pack both respell.
A draft without a reading answers empty.

### `LLanguageAccentRead`

One pronunciation as the reading view prints it, its reading resolved by the draft's own rule.
The caller passes the pack's answer for the switch, so it asks once for every row.
No pronunciation answers a blank row.

### `LLanguageCandidateRead`

The text a found reading shows, by the same rule a stored pronunciation uses.
A shown respelling wins, and a blank one falls back to the phonetic text.
A missing text reads as blank.

### `LLanguageTranscriptionRead`

The filled transcription rows a reading view lists, by Core's `LGlyphOtherRead`.
The glyph section is the one the draft's pack declares, and a blank language has none.

### `LLanguageGlyphRead`

The draft's transcription rows split by its pack's glyph section, by Core's `LGlyphBlockRead`.
An editor lists both sides, blank rows included.

### `LLanguageGlyphDivide`

The cells of the draft's glyph row, empty when its pack declares no glyph section.
The engine reaches the Core division only through this clerk.

### `LLanguageGlyphLoad`

The glyph section a pack declares, or none for a null or blank language.
It is the one glyph loader, shared by the draft readers here and the engine.

### `LLanguageGuiseScan`

One guise per reflex row, read by its trimmed language, the one owner of respelling, slashes and folding.
Each guise carries the row's shown text, applied once through `LReflexGuise.LReflexGuiseRead`.
The entry page, the series page and both Outpost notes all read their guises here.

### `LLanguageContourRead`

The tone contour syllables of a reading, or nothing when the contour would stay hidden.
A blank or non-tonal language draws no contour, even over a reading with tone marks.
A reading whose syllables carry no tone draws nothing either.

### `LLanguageContourScale`

The contour's pitch levels from the highest down, handed up so no outer layer writes the scale again.

### `LLanguageFlagFind`

The stored flag image path, or null when the name is blank or no file is stored.
`LLiveryFacade.LEngineLiveryRead` calls it, so a Joplin push never fetches a flag.
`LVarietyFlagScan` gives the stored path of each variety in an accent sheet, keyed by variety name.
It answers none when the sheet is not flagged.
A variety the language does not declare, or whose flag file is missing, is left out.

### `LLanguageFlagRead`

The flag image path of a language, fetched when the code is remote.
`LVarietyFlagRead` does the same for one variety, or null when the variety declares none.

### `LEnsignFind`

The stored path for one flag code, or null for a blank code.
A code under a trail root answers itself only when the file exists.
Any other code asks `LLanguageVault.LLanguageFlagFind`, which never fetches.

### `LLanguageFlagResolve`

A rooted code is a local file and answers itself when it exists.
Any other code is fetched through the language port.

## Stamp

Commit: `c2f335c331fe`
- `LLanguageCache.cs` `d16ef3eec7823894`
- `LLanguageClerk.cs` `9b2b5e191d8385d2`
