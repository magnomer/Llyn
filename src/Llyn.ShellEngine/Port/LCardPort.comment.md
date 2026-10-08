# LCardPort.cs
Hash: `2eddd17ffa961287`

## `public interface LCardPort`

The slice of the engine a deportment sees when it reads the links around a shown entry's cards.
`LCardFacade` implements it.

## `IReadOnlyList<LUsage> LEngineIncomingRead(long entryId);`

Every card of either kind that links to the entry, with the epithet the settings ask for.

## `IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LEngineTranslationRead(LEntryDraft shown);`

The link targets of every meaning and collocation of the shown entry, keyed by card id.
Every card of the entry has a key, so a reader never checks for a missing card.

## `LEtymologyResult LEngineEtymologyRead(LEntryDraft draft);`

The etymology of the shown entry, its etymons resolved to their stored entries.
