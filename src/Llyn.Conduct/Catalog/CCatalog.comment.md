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

## `public CSentenceOrder CCatalogOrderRead(string language)`

Where a language puts its particles and its dependents in a sentence.

## `internal static CSentenceOrder CCatalogOrderRead(LSentenceOrder order)`

Maps the engine's order into the Conduct shape, holding no rule.
The sentence gates hand it the engine's answer unread, so they name no engine record.

## `public IReadOnlyList<string> CCatalogSchemeRead(string language)`

The transcription schemes a language offers.

## `public IReadOnlyList<CSpeechValue> CCatalogSpeechRead(string language)`

The parts of speech a language offers, each with its id and name.

## `public CSpeechValue? CCatalogSpeechAdd(string language, string name)`

Adds a part of speech to a language, and answers nothing when the engine declines the name.

## `public CGlyph? CCatalogGlyphRead(string language)`

The glyph set a language writes in, or nothing when it has none.

## `public long CCatalogGlyphResolve(string character, string language)`

The entry a character stands for in a language, by id.

## `public IReadOnlyList<string> CCatalogLanguageRead()`

The lexicon languages the workspace knows.

## `public string CCatalogGlossRead()`

The language a new translation starts in, as the engine resolves it from the settings and the loaded packs.

## `public Task CCatalogEnsignLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Loads the cached flags, handing `store` each row as the Conduct shape.

## `public Task CCatalogEnsignLoad(`

Loads the flags of one language's `varieties` and hands the rows to `store` as Conduct rows.

## `public IReadOnlyList<CMeaning> CCatalogMeaningSort(long entryId)`

The Meanings of one Entry as sense-menu rows in reading order, each with its depth.
The engine returns the Meanings grouped by parent, and the menu wants them in reading order.
So the children of each parent are picked out and sorted by position before their own children follow.
The walk keeps its own path, so a deep tree never grows the call stack.
A Meaning is named by its title, or by its definition when it has no title.
One with neither reads the engine's text for `Display.Unknown`, so every driver shows the same word.
A row naming itself as its parent is skipped rather than followed, so it cannot loop the walk.

## `public (int, int)? CCatalogEntryLoad(long id)`

How many meaning and collocation cards a stored entry holds, children included.
It answers nothing when no entry has `id`.
The customs window shows the two counts as what a Replace would drop.

## `public IReadOnlyList<long> CCatalogMarkupFind(string headword, string language)`

The stored entries a markup entry could stand for, by id.

## `internal static IReadOnlyList<CEnsignRow> CCatalogEnsignRead(IReadOnlyList<LEnsignRow> rows)`

Maps the engine's flag rows into Conduct rows.
The reading view's flag load shares it, so the map has one owner.
