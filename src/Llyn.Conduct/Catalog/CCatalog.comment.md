# CCatalog.cs

## `public sealed class CCatalog`

The reference reads every driver shares: fonts, rules, orders, schemes, parts of speech, glyphs and languages.
It also sorts an entry's meanings and finds the entries a markup line names.
It maps each engine record into its Conduct record, so a driver never names a Core type.
It stands on the atelier's ports, and `CAtelier` hands one out on each read.
It holds no state of its own.

## `internal CCatalog(CAtelier atelier)`

Only the atelier builds it, over its own ports.

## `public CFont CCatalogFontRead(string language, CFontRole role)`

The font a language shows `role` in, read through `LCatalogFontRead`.

## `internal static CFont LCatalogFontRead(LSettingsPort settings, string language, CFontRole role)`

The one font rule, shared by the catalog and the reading view's sound area.
An unsized font carries no size.
A blank language or a refused read answers the font with nothing set, so the surface keeps its theme.

## `private static LFontRole LCatalogRoleRead(CFontRole role)`

The engine role of the same name, switched name by name.
An unknown role throws, so a role added on one side alone fails loudly.

## `public IReadOnlyList<CFont> CCatalogFontRead(string language, IReadOnlyList<CFontRole> roles)`

The fonts of several roles in one read, in the order the roles are given.
A surface showing two roles together, such as an example and its gloss, asks once.

## `internal static CSentenceOrder CCatalogOrderRead(LSentenceOrder order)`

Maps the engine's order into the Conduct shape, holding no rule.
The sentence frame hands it the engine's answer unread, so it names no engine record.

## `public IReadOnlyList<string> CCatalogSchemeRead(string language)`

The transcription schemes a language offers.

## `public IReadOnlyList<CSpeechValue> CCatalogSpeechRead(string language)`

The parts of speech a language offers, each with its id and name.

## `public CGlyph? CCatalogGlyphRead(string language)`

The glyph set a language writes in, or nothing when it has none.

## `internal static long? LCatalogGlyphOpen(CEnvoy envoy, LSettingsPort settings, Func<long> resolve)`

The one failure owner of every glyph chip that opens a character's entry.
`resolve` is the gate's one engine call, which finds or makes the entry.
A refused resolve shows `Glyph.OpenFailed` through the panel's envoy and answers null.
`settings` reads the ready notice, as `CLedger.LLedgerFailureShow` does for every gate.

## `public IReadOnlyList<string> CCatalogLanguageRead()`

The lexicon languages the workspace knows.

## `public string CCatalogGlossRead()`

The language a new translation starts in, as the engine resolves it from the settings and the loaded packs.

## `public Task<IReadOnlyList<string>> CCatalogEnsignLoad(`

Loads the cached flags, handing `store` each row as the Conduct shape.
It answers the loaded languages, so a language menu fills from the load that flagged it.

## `public Task CCatalogEnsignLoad(`

Loads the flags of one language's `varieties` and hands the rows to `store` as Conduct rows.

## `public IReadOnlyList<CMeaning>? CCatalogMeaningRead(long entryId, CEnvoy envoy)`

The Meanings of one Entry as sense-menu rows, ready in reading order with their depth.
The order and the name fallback are the engine's, and this read only maps the rows by name.
It chooses the fallback key `Display.Unknown`, which the engine words.
A failed read shows `Mention.FindFailed` through `envoy` and answers null, so no menu opens.

## `public (int, int)? CCatalogEntryLoad(long id)`

How many meaning and collocation cards a stored entry holds, children included.
It answers nothing when no entry has `id`.
The customs window shows the two counts as what a Replace would drop.

## `public IReadOnlyList<long> CCatalogMarkupFind(string headword, string language)`

The stored entries a markup entry could stand for, by id.

## `internal static IReadOnlyList<CEnsignRow> CCatalogEnsignRead(IReadOnlyList<LEnsignRow> rows)`

Maps the engine's flag rows into Conduct rows.
The reading view's flag load shares it, so the map has one owner.
