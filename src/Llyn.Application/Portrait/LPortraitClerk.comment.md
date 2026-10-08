# LPortraitClerk.cs
Hash: `a540ae07c3d4080b`

## `public sealed class LPortraitClerk`

The entry page composed for display.
Saving and printing a composed page live in `LPortraitClerkPress`.
A failing read of any section throws to the caller.
No section is silently left out of the page.
The kind pages of examples, references and situations are composed by their own clerks.

## `public LPortraitClerk(LLanguageClerk languages, LEntryClerk entries, LVocabularyClerk vocabulary, LTranslationClerk translations, LReferenceClerk references, LFavoriteClerk favorites, LFanqieClerk fanqie, LFrequencyClerk frequencies, LParadigmClerk paradigms, LScriptClerk scripts, Func<LSettings> settings)`

Keeps the clerks each section reads through.

## `public LPortraitPage LPortraitClerkRead(long entryId, LPortraitLabel label, bool fetch = true)`

The page of one entry, refused when the entry no longer stands.
Sections come in display order, then the readings with respelling and phonemic marks decided per language.
The speech chips open with the name of the entry's lexical unit when one is chosen.
A failing favorite check throws rather than showing the entry as no favorite.
A false `fetch` reads stored frequencies only, so a push over every entry starts no network fetch.

## `private IReadOnlyDictionary<long, LPortraitLink> LPortraitTargetScan(IReadOnlyList<long> ids)`

The translation targets as links, keyed by entry id.
A failing read throws.

## `private IReadOnlyDictionary<long, string> LPortraitSourceScan()`

The citation names by reference id.
A failing read throws.

## `private IReadOnlyList<LFanqieRow> LPortraitFanqieScan(long entryId, string language)`

The fanqie rows for a language with books, empty otherwise.
A failing read throws.

## `private LSentenceOrder LPortraitOrderRead(string language)`

The sentence order of the language.
A failing read throws.

## `private LGlyph? LPortraitGlyphRead(string language)`

The glyph of the language, null for a blank language.
A failing read throws.

## `private IReadOnlyList<LFrequency> LPortraitFrequencyScan(long entryId, bool fetch)`

The frequency rows, starting a fetch for an entry without any only when `fetch` allows.
A failing read throws.

## `private IReadOnlyList<LParadigmSlot> LPortraitParadigmScan(long entryId)`

The paradigm slots as shown.
A failing read throws.

## `private IReadOnlyList<LUsage> LPortraitIncomingScan(long entryId)`

The incoming translations.
A failing read throws.

## `private IReadOnlyList<LScriptImage> LPortraitScriptScan(long entryId, string language)`

The script images for a language with styles, empty otherwise.
A failing read throws.
