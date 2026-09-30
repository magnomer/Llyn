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

## `public CGlyph? CCatalogGlyphRead(string language)`

The glyph set a language writes in, or nothing when it has none.

## `internal static long? LCatalogGlyphOpen(CEnvoy envoy, LSettingsPort settings, Func<long> resolve)`

The one failure owner of every glyph chip that opens a character's entry.
`resolve` is the gate's one engine call, which finds or makes the entry.
A refused resolve shows `Glyph.OpenFailed` through the panel's envoy and answers null.
`settings` reads the ready notice, as `CLedger.LLedgerFailureShow` does for every gate.

## `public IReadOnlyList<string> CCatalogLanguageRead()`

The lexicon languages the workspace knows.


## `public Task<IReadOnlyList<string>> CCatalogEnsignLoad(`

Loads the cached flags, handing `store` each row as the Conduct shape.
It answers the loaded languages, so a language menu fills from the load that flagged it.

## `public Task CCatalogEnsignLoad(`

Loads the flags of one language's `varieties` and hands the rows to `store` as Conduct rows.

## `internal static IReadOnlyList<CMeaning> LCatalogMeaningRead(`

Maps the engine's Meaning rows to sense-menu rows by name, with no rule of its own.
The shared sense read in `CMention` uses it.

## `public static CArticulation CCatalogConsonantRead()`

The IPA consonant chart of the input aid, ready to build.
It needs no session, so a driver reads it while it builds the aid.

## `public static CArticulation CCatalogVowelRead()`

The IPA vowel chart of the input aid, ready to build.
It needs no session, so a driver reads it while it builds the aid.

## `private static CArticulation LCatalogArticulationRead(LArticulation chart)`

Chooses the localization key of every header and side, and passes the symbols on unread.

## `internal static IReadOnlyList<CEnsignRow> CCatalogEnsignRead(IReadOnlyList<LEnsignRow> rows)`

Maps the engine's flag rows into Conduct rows.
The reading view's flag load shares it, so the map has one owner.
