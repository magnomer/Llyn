# TInterfaceCatalog.cs
Hash: `18eb67a5e0766ebf`

## `internal static partial class TInterface`

The relay for the browsing seam: the find call of each browsed kind, and the catalog vocabulary.
Each relay delegates to one engine or Core operation, or builds one row, and returns the result.
`TCardChildSet` is one exception, since it builds the card itself.
`TEngineCitationCreate` is the other, which creates the reference, raises a reference bulletin and returns the stored reference.
The orderings and the matches under test are reached only here, so no test body calls them itself.

## `internal static LCatalogRegister TCatalogRegisterCreate(LRegister register, int usage)`

Builds one browsed Register row from a stored Register, so a test reads the row's ready answers.

## `internal static LRegisterOffer TEngineRegisterFind(this LEngine engine, string text, string language, long card)`

Reads the offer a card's register field shows for a typed text, with no draft held.
Without a draft no card already holds a Register.
So every match is offered, and only the order and the limit decide the rows.

## `internal static IReadOnlyList<LEntry> TCatalogEntrySort(IReadOnlyList<LEntry> entries, LCatalogOrder order)`

Orders entries as the entry catalog does, so a test pins the tie rule on rows it builds.

## `internal static IReadOnlyList<LCatalogPronunciation> TCatalogPronunciationSort(IReadOnlyList<LCatalogPronunciation> rows, LCatalogOrder order)`

Orders pronunciation rows as the phonology catalog does.

## `internal static IReadOnlyList<LCatalogFavorite> TCatalogFavoriteSort(IReadOnlyList<LCatalogFavorite> favorites, LCatalogOrder order)`

Orders marked entries as the favorite catalog does.

## `internal static IReadOnlyList<LCatalogExample> TCatalogExampleSort(IReadOnlyList<LCatalogExample> rows, LCatalogOrder order)`

Orders Example rows as the Example catalog does.

## `internal static IReadOnlyList<LCatalogSituation> TCatalogSituationSort(IReadOnlyList<LCatalogSituation> rows, LCatalogOrder order)`

Orders Situation rows as the Situation catalog and its offer do.

## `internal static IReadOnlyList<LCatalogReference> TCatalogReferenceSort(IReadOnlyList<LCatalogReference> rows, LCatalogOrder order)`

Orders Source rows as the Source catalog and its citation offer do.

## `internal static IReadOnlyList<LCatalogRegister> TCatalogRegisterSort(IReadOnlyList<LCatalogRegister> rows, LCatalogOrder order)`

Orders Register rows as the Register catalog and its offer do.

## `internal static LCatalogExample TCatalogExampleCreate(LExample stored, string source, int usage)`

Builds one browsed Example row, so a sort test builds its rows without the engine.

## `internal static LCatalogFavorite TCatalogFavoriteCreate(LEntry entry, string marked)`

Builds one marked entry row with its marking time.

## `internal static LCatalogPronunciation TCatalogPronunciationCreate(LEntry entry, string sound)`

Builds one phonology row of an entry and its sound.

## `internal static LCatalogReference TCatalogReferenceCreate(LReference stored, string name, string byline, int usage)`

Builds one browsed Source row with no credited author.

## `internal static LCatalogSituation TCatalogSituationCreate(LSituation stored, int usage)`

Builds one browsed Situation row with its usage count.
