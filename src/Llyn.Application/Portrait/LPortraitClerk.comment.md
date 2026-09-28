# LPortraitClerk.cs

## `public sealed class LPortraitClerk`

The entry page composed for display, export and print.
Every side reading is guarded, so a failing fetch or vault leaves a section out rather than a page unrendered.
The kind pages of examples, references and situations are composed by their own clerks.

## `public LPortraitClerk(LRig rig, LLanguageClerk languages, LEntryClerk entries, LVocabularyClerk vocabulary, LTranslationClerk translations, LReferenceClerk references, LFavoriteClerk favorites, LFanqieClerk fanqie, LFrequencyClerk frequencies, LParadigmClerk paradigms, LScriptClerk scripts, LMarkupClerk markup, Func<LSettings> settings)`

Reads the portrait port and the press out of `rig` and keeps the clerks each section reads through.

## `public LPortraitPage LPortraitClerkRead(long entryId, LPortraitLabel label)`

The page of one entry, refused when the entry no longer stands.
Sections come in display order, then the readings with respelling and phonemic marks decided per language.

## `public async Task LPortraitClerkExport(LPortraitPage portrait, string path, LPortraitMedium format)`

A PDF goes through the press as rendered sheet HTML, every other format through the portrait port.

## `public void LPortraitMarkupExport(long entryId, string path)`

The entry written as markup through the markup clerk.

## `public Task LPortraitClerkPrint(LPortraitPage page, LPressTicket ticket)`

The page rendered as sheet HTML and handed to the press with the ticket.

## `public static LPressTicket LPortraitTicketCreate(string printer, double? width, double? height, bool landscape, int copies, bool collated, LPressSide side, LPressInk ink)`

The ticket a print dialog's answer stands for.
A dialog that named no usable sheet size prints on the local sheet.

## `public static LPortraitLabel LPortraitLabelCreate(IReadOnlyList<string> words)`

The label an entry page is worded with, taken word by word in the record's order.
It throws unless every one of the label's words is given.

## `public static LPortraitLegend LPortraitLegendCreate(IReadOnlyList<string> words, IReadOnlyDictionary<string, string> kinds)`

The legend a catalog page is worded with, taken word by word in the record's order.
Each Source kind is worded by its stored word, and a kind the words leave out keeps its own name.

## `private IReadOnlyDictionary<long, LPortraitLink> LPortraitTargetScan(IReadOnlyList<long> ids)`

The translation targets as links, empty when the read fails.

## `private IReadOnlyDictionary<long, string> LPortraitSourceScan()`

The citation names by reference id, empty when the read fails.

## `private IReadOnlyList<LFanqieRow> LPortraitFanqieScan(long entryId, string language)`

The fanqie rows for a language with books, empty otherwise or on failure.

## `private LSentenceOrder LPortraitOrderRead(string language)`

The sentence order of the language, the default on failure.

## `private LGlyph? LPortraitGlyphRead(string language)`

The glyph of the language, null for a blank language or on failure.

## `private IReadOnlyList<LFrequency> LPortraitFrequencyScan(long entryId)`

The frequency rows, empty on failure.

## `private IReadOnlyList<LParadigmSlot> LPortraitParadigmScan(long entryId)`

The paradigm slots as shown, empty on failure.

## `private IReadOnlyList<LUsage> LPortraitIncomingScan(long entryId)`

The incoming translations, empty on failure.

## `private IReadOnlyList<LScriptImage> LPortraitScriptScan(long entryId, string language)`

The script images for a language with styles, empty otherwise or on failure.
