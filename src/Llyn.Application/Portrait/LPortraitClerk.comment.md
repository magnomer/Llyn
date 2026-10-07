# LPortraitClerk.cs
Hash: `2d689e9b18f16a53`

## `public sealed class LPortraitClerk`

The entry page composed for display, export and print.
A failing read of any section throws to the caller.
No section is silently left out of the page.
The kind pages of examples, references and situations are composed by their own clerks.

## `private const double LPortraitInch = 96.0;`

The device-independent pixels in one inch, the unit a print dialog measures a sheet in.

## `public LPortraitClerk(LRig rig, LLanguageClerk languages, LEntryClerk entries, LVocabularyClerk vocabulary, LTranslationClerk translations, LReferenceClerk references, LFavoriteClerk favorites, LFanqieClerk fanqie, LFrequencyClerk frequencies, LParadigmClerk paradigms, LScriptClerk scripts, LMarkupClerk markup, Func<LSettings> settings)`

Reads the portrait port and the press out of `rig` and keeps the clerks each section reads through.

## `public LPortraitPage LPortraitClerkRead(long entryId, LPortraitLabel label, bool fetch = true)`

The page of one entry, refused when the entry no longer stands.
Sections come in display order, then the readings with respelling and phonemic marks decided per language.
The speech chips open with the name of the entry's lexical unit when one is chosen.
A failing favorite check throws rather than showing the entry as no favorite.
A false `fetch` reads stored frequencies only, so a push over every entry starts no network fetch.

## `public async Task LPortraitClerkExport(LPortraitPage portrait, string path, LPortraitMedium format)`

A PDF goes through the press as rendered sheet HTML, every other format through the portrait port.

## `public void LPortraitMarkupExport(long entryId, string path)`

The entry written as markup through the markup clerk.

## `public Task LPortraitClerkPrint(LPortraitPage page, LPressTicket ticket)`

The page rendered as sheet HTML and handed to the press with the ticket.

## `public static LPressTicket LPortraitTicketCreate(string printer, double? width, double? height, bool landscape, int copies, bool collated, LPressSide side, LPressInk ink)`

The ticket a print dialog's answer stands for.
The dialog measures the sheet in device-independent pixels, and the paper takes inches.
A dialog that named no usable sheet size prints on the local sheet.
A width or height that is missing or not above zero is no usable size.

## `public static IReadOnlyList<(LPortraitMedium, string, bool)> LPortraitMediumRead()`

Every format an entry can be exported to, in the order a chooser offers them.
Each row carries the file suffix the format is written under, and whether it is the default.
HTML is the default, since every reader can open it.

## `public static IReadOnlyList<string> LPortraitKindRead()`

The stored word of every Source kind, in the kind's own order.
A legend is worded per kind by these words, so no caller names the kinds.

## `public static LPortraitLabel LPortraitLabelCreate(IReadOnlyList<string> words)`

The label an entry page is worded with, taken word by word in the record's order.
The last four words name the units Content, Function, Morpheme and Word, in that order.
It throws unless every one of the label's words is given.

## `public static LPortraitLegend LPortraitLegendCreate(IReadOnlyList<string> words, IReadOnlyDictionary<string, string> kinds)`

The legend a catalog page is worded with, taken word by word in the record's order.
Each Source kind is worded by its stored word, and a kind the words leave out keeps its own name.

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
