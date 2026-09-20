# LLanguageCache.cs

## `public sealed class LLanguageCache`

The loaded packs of one rig, read once per language and kept.
The language vault parses its file on every read, so whoever reads often must cache.
The engine and the draft clerk share one instance, so a pack is parsed once for both.
A new rig gets a new cache, since its packs may differ.
The derivations that need only a pack live here too.
Those are the respelling of a reading and the anatomy of a reflex.
They run on every seam that writes such a row, so a view only reads what is stored.

## `public LLanguageCache(LLanguageVault vault)`

Binds the cache to the vault it reads a pack from on the first ask.

## `public LLanguage LLanguageCacheRead(string language)`

The pack named by `language`, read once and answered from memory after.
A blank name is an argument error, since no pack has no name.
The lock is the cache's own, so a caller holding another gate never waits on this one.

## `public LPronunciationDraft LLanguageRespellingResolve(string language, LPronunciationDraft spoken)`

Fills the row's respelling from its reading through the pack's respelling groups, scoped by its variety.
It runs whether or not the respelling switch is on, so both forms are always stored.
The switch then only picks which form is shown.
A blank reading, a blank language or a pack without groups leaves the respelling blank.
The form then shows the reading in its place.
Whatever the user wrote into the respelling by hand is replaced, because a stale respelling would silently mislead.

## `public LReflexDraft LLanguageRespellingResolve(LReflexDraft row)`

Fills the row's respelling from its reading through the respelling groups of the row's own language.
The respelling is as bare as the reading, and the view draws the slashes of a phonemic language around it.
A blank reading, a blank language or a language without groups leaves the respelling blank.
The groups are asked with no variety, so only the unscoped ones apply.

## `public LEntryDraft LLanguageRespellingRebuild(LEntryDraft content)`

Derives every pronunciation row's respelling again, for a draft whose language changed.

## `public LEntryDraft LLanguageRespellingUpdate(LEntryDraft draft)`

Fills only the blank respellings of a draft, and recuts every reflex row.
A row the user already respelled keeps what it has, since this runs on a draft being stored, not edited.

## `public LReflexDraft LLanguageAnatomyResolve(string language, LReflexDraft row)`

The row with its anatomy cut afresh under the rules of the pack named by `language`, the entry's language.
A blank language, a pack without rules or a reflex language no rule names leaves the anatomy empty.

## `public IReadOnlyList<LReflexDraft> LLanguageAnatomyScan(string language, IReadOnlyList<LReflexDraft> rows)`

Every row resolved in turn, order and ids kept.

## `public LEntryDraft LLanguageAnatomyRebuild(LEntryDraft content)`

The draft with every reflex row resolved under its current language, run when the language changes.
