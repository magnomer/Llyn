# CCatalog.cs
Hash: `ebd60b88daa0f841`

## `public sealed class CCatalog`

The reference reads every driver shares.
They cover fonts, sentence orders, glyphs, languages, flags, meanings and articulation charts.
It maps each engine record into its Conduct record, so a driver never names a Core type.
It stands on the atelier's ports, and `CAtelier` hands one out on each read.
It holds no state beyond the atelier.

## `internal CCatalog(CAtelier atelier)`

Only the atelier builds it, over its own ports.

## `internal static CFont LCatalogFontRead(LSettingsPort settings, string language, CFontRole role)`

The one font rule, shared by every area that reads a pack's typography in its own language.
An unsized font carries no size.
A blank language answers the font with nothing set, so the surface keeps its theme.
A refused read is not caught here and reaches the caller.
The engine catches the pack failure it expects lower down.

## `private static LFontRole LCatalogRoleRead(CFontRole role)`

The engine role of the same name, switched name by name.
An unknown role throws, so a role added on one side alone fails loudly.

## `internal static async Task<CEnsignSheet<LCatalogKind>> LCatalogEnsignLoad<LCatalogKind>(LSettingsPort settings, Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store, Func<LCatalogKind> read)`

The one ordering rule for an area's rows that show flags.
The flag fill runs first, then the area's read.
The fill runs into the driver's store, so each row painted afterwards finds its flag drawn.
The settings port already answers the languages without flags when a flag file cannot be written or read.
Every other failure is not caught here and reaches the caller, which routes it.
The panels' flag loads reach it, so no driver orders the two engine calls itself.

## `internal static CSentenceOrder CCatalogOrderRead(LSentenceOrder order)`

Maps the engine's order into the Conduct shape, holding no rule.
The sentence frame hands it the engine's answer unread, so it names no engine record.

## `internal static long? LCatalogGlyphOpen(CEnvoy envoy, LSettingsPort settings, Func<long> resolve)`

The one failure owner of every glyph chip that opens a character's entry.
`resolve` is the gate's one engine call, which finds or makes the entry.
A refused resolve shows `Glyph.OpenFailed` through the panel's envoy and answers null.
`settings` reads the ready notice, as `CLedger.LLedgerFailureShow` does for every gate.

## `public IReadOnlyList<string> CCatalogLanguageRead()`

The lexicon languages the workspace knows.

## `public Task<IReadOnlyList<string>> CCatalogEnsignLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Loads the cached flags, handing `store` each row as the Conduct shape.
It answers the loaded languages, so a language menu fills from the load that flagged it.

## `internal static IReadOnlyList<CMeaning> LCatalogMeaningRead(IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> rows)`

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
The errand, display and timbre flag loads share it, so the map has one owner.

## `internal static CCatalogOrder LCatalogOrderRead(LCatalogOrder order)`

The Conduct mirror of an engine ordering, member for member.
It maps each member by name, never by cast, so a reordered enum cannot shift a meaning.
An unknown member is a caller's error.

## `internal static LCatalogOrder? LCatalogOrderRead(CCatalogOrder? order)`

The engine ordering a driver's choice stands for, or null when the driver chose none.

## `internal static LCatalogOrder LCatalogOrderRead(CCatalogOrder order)`

The by-name map from a Conduct ordering to the engine's, shared by the panels and the atelier.

## `internal static CCatalogFilter LCatalogFilterRead(LCatalogFilter filter)`

The Conduct copy of an engine filter, carrying its hidden languages.

## `internal static LSubject LCatalogSubjectRead(CSubject subject)`

The engine subject a driver's subject names, mapped member by member by name like the ordering.

## `internal static CVistaRow LCatalogRowRead(LVistaRow row)`

The Conduct copy of one entry a vista lists, carrying its chosen flag.
Every entry list and the translation offer map through here, so the copy has one home.
It copies the engine's empty epithet as it stands, so the map holds no fallback.
