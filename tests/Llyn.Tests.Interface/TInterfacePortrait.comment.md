# TInterfacePortrait.cs
Hash: `11a523f8150fe7ed`

## `internal static partial class TInterface`

The relay for the export surface and the Joplin livery.
Tests call production operations only through here, so a renamed operation breaks in one file.

## `internal static LPortraitLabel TPortraitLabelRead()`

A fixed English label set, so a test asserts on known words without the localization files.
Its unit names are English too.

## `internal static LPortraitLink TPortraitLinkCreate(string headword, string language)`

Builds a link record, handing its arguments to the constructor unchanged.

## `internal static LPortraitMedia TPortraitMediaCreate(string location, string span)`

Builds a media record, handing its arguments to the constructor unchanged.

## `internal static LTheme TThemeLoad()`

Loads the shipped theme through `LThemeLoader.LThemeLoaderLoad`.

## `internal static string TThemeRead(this LTheme theme, string name)`

Relays `LTheme.LThemeRead`, so a test compares a colour with the role it should come from.

## `internal static string TSheetFormat(LPortraitPage page, LTheme theme)`

Relays `LSheet.LSheetFormat`.

## `internal static string TOutlineFormat(LPortraitPage page)`

Relays `LOutline.LOutlineFormat`.

## `internal static LLiveryNote TLiveryFormat(LLiveryPage page, Func<string, string> lookup, Func<long, string>? note = null, Func<string, string, string, string>? link = null)`

Writes a Joplin entry body through `LLiverySheet.LLiveryFormat` over the loaded theme.
It passes a fixed style id and the `note` it is handed.
With no `note`, every entry id stands as its own note id.
It passes the `link` it is handed, or one that answers empty, so every chip stays plain by default.
It answers the whole note, so a test reads the body and the parcels.

## `internal static LLiveryNote TLiveryFormat(LLiveryStem stem, Func<string, string> lookup, Func<long, string>? note = null)`

Writes a series note through the matching `LLiverySheet.LLiveryFormat` overload.
It passes the same style id and the same default `note` as the entry overload.

## `internal static LLiveryNote TLiveryFormat(LLiveryDiwei diwei, Func<string, string> lookup, Func<long, string>? note = null)`

Writes a rime-table category note through the matching `LLiverySheet.LLiveryFormat` overload.
It passes the same style id and the same default `note` as the entry overload.

## `internal static LLiveryNote TLiveryFormat(LLiveryLanguage language, Func<string, string> lookup, Func<long, string>? note = null)`

Writes a language's sound note through the matching `LLiverySheet.LLiveryFormat` overload.
It passes the same style id and the same default `note` as the entry overload.

## `internal static Func<long, string> TCourierNoteBuild(string stamp, IReadOnlyList<LEntry> entries)`

Builds the courier's `note` map through `LCourierNote.LCourierNoteBuild`.
It hands over a `LLiverySheet` over the loaded theme, so the ids match a real push.

## `internal static Task<LReceipt> TCourierSend(LEngine engine, Func<long, LLiveryPage?> page, Func<string, LLiveryLanguage> language)`

Runs one push through the engine's courier clerk with the readers it is handed.
A test can then shape the entry pages and the language reads without fetching anything.
Its `lookup` answers each key itself, so a test finds notebooks and notes by their keys.

## `internal static LLiveryLanguage TLiveryLanguageBuild(string name, IReadOnlyList<LCatalogPronunciation> pronunciation, IReadOnlyList<LLiveryStem> stem, IReadOnlyList<LLiveryDiwei> diwei)`

Builds a language read, handing its arguments to the constructor unchanged.
A push test hands it to `TCourierSend`, so it chooses which reconstruction notes the language carries.

## `internal static LLiveryStem TLiveryStemCreate(LStemPage page, IReadOnlyList<LEntry> entry)`

Builds a series read, handing its arguments to the constructor unchanged.

## `internal static LStemPage TStemPageCreate(string language, string key, IReadOnlyList<string> characters)`

Builds a series page, handing its arguments to the constructor unchanged.

## `internal static LLiveryDiwei TLiveryDiweiCreate(string kind, string language, string key, IReadOnlyList<LEntry> entry)`

Builds a rime-table category read whose page holds no sections.
A push test needs only the kind and key, since those place the note and address the entry chip.

## `internal static LDiweiSection TDiweiSectionCreate(string label, IReadOnlyList<LDiweiLine> lines, IReadOnlyList<LTallyRow> tallies, bool switched, bool respelled)`

Builds a category section, handing its arguments to the constructor unchanged.

## `internal static LDiweiLine TDiweiLineCreate(string reading, string label, bool rounded, int rank, IReadOnlyList<string> characters)`

Builds a section line, handing its arguments to the constructor unchanged.

## `internal static LTallyMark TTallyMarkCreate(string text, IReadOnlyList<string> characters)`

Builds a tally mark, handing its arguments to the constructor unchanged.

## `internal static LTallyRow TTallyRowCreate(string language, string kind, IReadOnlyList<LTallyMark> marks)`

Builds a tally row, handing its arguments to the constructor unchanged.

## `internal static LTheme TThemeCreate(IReadOnlyDictionary<string, string> colors)`

Builds a theme from `colors` alone, so a test controls every role `TLiveryStyleFormat` reads.

## `internal static string TLiveryStyleFormat(LTheme theme)`

Relays `LLiveryStyle.LLiveryStyleFormat`.

## `internal static LTranslationTarget TTranslationTargetCreate(long id, string headword, string language)`

Builds a translation target, handing its arguments to the constructor unchanged.

## `internal static LUsage TUsageCreate(long id, LOwner owner, long entryId, string headword, string language, LStateValue title)`

Builds an incoming usage, handing its arguments to the constructor unchanged.

## `internal static LGlossDraft TGlossDraftCreate(long id, string language, LStateValue text)`

Builds a gloss draft, handing its arguments to the constructor unchanged.

## `internal static LSentenceDraft TSentenceDraftCreate(LExampleDraft example)`

Wraps an example with an unspecified particle and dependence.

## `internal static LEtymologyDraft TEtymologyDraftCreate(string text, IReadOnlyList<LMentionDraft> mentions)`

Builds an etymology draft, handing its arguments to the constructor unchanged.

## `internal static LFanqieGroup TFanqieGroupCreate(string heading, string label, string source, IReadOnlyList<LFanqieRow> rows, IReadOnlyList<string> stems)`

Builds a rime group, handing its arguments to the constructor unchanged.

## `internal static LScriptImage TScriptImageCreate(string character, string style, int position, string caption, string gloss, byte[] data)`

Builds a script image, handing its arguments to the constructor unchanged.

## `internal static LScriptGroup TScriptGroupCreate(string heading, string style, string gloss, IReadOnlyList<LScriptImage> images)`

Builds a script group, handing its arguments to the constructor unchanged.

## `internal static string TSheetNormalize(string? text)`

Relays `LSheet.LSheetNormalize`.

## `internal static string TOutlineNormalize(string? text)`

Relays `LOutline.LOutlineNormalize`.

## `internal static string TFolioLineFormat(string? text)`

Relays `LFolioLine.LFolioLineFormat`.

## `internal static LPortraitLegend TPortraitLegendRead()`

A fixed English legend with no reference kind names, so a page read needs no localization files.

## `internal static IReadOnlyList<string> TPortraitTextRead(LPortraitPage page)`

Lists every string a page shows, in reading order, children recursed.
A writer that carries the page whole prints every one of them.
So a coverage test reads its expectations here and never from the draft.

## `private static List<string> TPortraitTextRead(LPortraitSection section)`

Lists one section's strings, then its children's.
A band, card or kind prints its heading, so only those roles list it.
An image is a picture and not text, so its address and caption are not listed.
A video is a link drawn as text, so its address and span are.

## `private static void TPortraitTextAdd(List<string> shown, string text)`

Adds `text` unless it is empty, since an empty string shows nothing.

## `private static void TPortraitTextAdd(List<string> shown, IReadOnlyList<string> chips)`

Adds each chip through the string overload.

## `private static void TPortraitTextAdd(List<string> shown, IReadOnlyList<LPortraitLine> lines)`

Adds each line's label, then its text, through the string overload.

## `internal static LPortraitLine TPortraitLineCreate(string label, string text, string open = "", string close = "")`

Builds a page line, handing its arguments to the constructor unchanged.

## `internal static LPortraitSection TPortraitSectionCreate(string heading, IReadOnlyList<LPortraitLine> line, string note, IReadOnlyList<LPortraitMedia> image, IReadOnlyList<LPortraitMedia> video)`

Builds a section with a note, leaving every later member at its default.

## `internal static LPortraitSection TPortraitSectionCreate(string heading, int position, IReadOnlyList<LPortraitLine> line, IReadOnlyList<string> chip, IReadOnlyList<LPortraitLink> link, IReadOnlyList<LPortraitMedia> image, IReadOnlyList<LPortraitMedia> video, IReadOnlyList<LPortraitSection> child, LPortraitRole role = LPortraitRole.LPortraitRoleBand)`

Builds a full section with an empty note, a band unless `role` says otherwise.

## `internal static LPortraitPage TPortraitPageCreate(string title, string language, IReadOnlyList<string> chip, IReadOnlyList<LPortraitSection> section)`

Builds a page, leaving the favorite flag and the lines at their defaults.

## `internal static LPortraitPage TPortraitPageCreate(string title, string language, bool favorite, IReadOnlyList<LPortraitLine> line, IReadOnlyList<string> chip, IReadOnlyList<LPortraitSection> section)`

Builds a page with its favorite flag and lines as well.

## `internal static LPressTicket TPressTicketCreate(string printer, bool landscape, int copies)`

Builds a press ticket on metric paper, collated, long-edge, in gray ink.
Only the printer, the orientation and the copies vary between tests.

## `internal static string TMarkdownNormalize(string? text)`

Relays `LMarkdown.LMarkdownNormalize`.

## `internal static IReadOnlyList<LMarkdownBlock> TMarkdownParse(string? text)`

Relays `LMarkdown.LMarkdownParse`.

## `internal static void TFolioSave(LPortraitPage page, LTheme theme, string path)`

Relays `LFolio.LFolioSave`, writing the page to `path`.
