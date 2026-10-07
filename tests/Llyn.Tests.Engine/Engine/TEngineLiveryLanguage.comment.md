# TEngineLiveryLanguage.cs
Hash: `784cce2be075dd04`

## `public sealed class TEngineLiveryLanguage`

Calls the language read of `LEngineLiveryRead` through the `TLiveryRead` relay, one fact per part.
The `LiveryFormat` facts write those reads as notes through the `TLiveryFormat` relay.

## `private const string TLiveryLanguagePack`

The pack `LiveryRead_FetchedSeries_CarriesItsPageAndEntries` loads, naming one fanqie book and a series rule.

## `private const string TLiveryLanguageBooks`

The book page the fake client serves to the fanqie fetch.

## `private const string TLiveryLanguageModule`

The series page the fake client serves to the series fetch.

## `private const string TLiveryLanguageBook = "Classical Chinese";`

A shipped language with a fanqie book, so its categories pass the book check.

## `private static readonly TimeSpan TLiveryLanguagePatience = TimeSpan.FromSeconds(5);`

How long `TLiveryLanguageSettle` waits for the fetch it watches.

## `private static readonly LHypothesis TLiveryLanguageTables`

The reconstruction tables `TLiveryLanguagePlace` hands to `TDiweiApply`, so the placed row also yields a tone.

## `public async Task LiveryRead_FetchedSeries_CarriesItsPageAndEntries()`

Saves one entry, starts its fanqie and series fetch, and waits for both to settle.
Reads the language and checks the one series page and its one entry.

## `public void LiveryRead_PlacedCharacter_ListsInitialRimeAndToneWithTheirKind()`

Places one character in the book language, then reads that language.
Checks the initial, rime and tone categories in that order, each with its kind, key and entry.

## `public void LiveryRead_RespellingAndTallyOn_SectionsFollowTheRespellingSetting()`

Reads the placed character's sections before and after turning respelling and the tally on.
The sections start unrespelled, and respelling must then be on for the book.
Every section then carries both flags pinned true, and the tally rows are present.
Only the initial and rime sections can hold them, since a reflex has no tone part to tally.

## `public void LiveryRead_LanguageWithoutBooksOrSeries_AnswersEmptyLists()`

Places a character under a pack with neither a fanqie book nor a series rule.
Reads that language and finds no series and no categories, but keeps its pronunciation row.

## `public void LiveryRead_EntriesOfTwoLanguages_KeepsOnlyTheAskedLanguageInHeadwordOrder()`

Saves two English entries and one Spanish entry, then reads English.
Checks that only the English rows remain, in headword order.

## `public void LiveryFormat_SeriesNote_LinksTheStoredEntryAndLeavesAnUnstoredCharacterPlain()`

Saves one entry and builds a series holding its character and one unstored character.
Writes the series note through `TLiveryFormat` and finds the stored character linked and the other plain.

## `public void LiveryFormat_RimeNote_PrintsSectionsInPageOrderWithReadingsAndTallies()`

Reads the placed character's rime category and swaps in two sections, the second holding a tally row.
Checks section order, each line's reading and label, the rounded mark and the tally in the second section.

## `public void LiveryFormat_ToneNote_PrintsTheToneLabelAndLinksThePlacedEntry()`

Reads the placed character's tone category and writes its note with a `Display.FanqieTone` pattern.
Checks the heading shows the formatted tone and the entry list links the placed entry.

## `public void LiveryFormat_SoundNote_HoldsOnlyItsLanguageRowsAndMarksAMissingSound()`

Saves an English and a Spanish entry without sounds, then writes the English sound note.
Checks the English row links its headword beside the escaped `[ ]` and the Spanish headword is absent.

## `private static LEntry TLiveryLanguagePlace(LEngine engine, TWorkspace workspace)`

Saves one entry in the book language with a Korean reflex, stores its fanqie row and applies the tables.
The anchor sits on a reflex row, so without the reflex no row is anchored and every category reads empty.
Anchors that reflex to the row so each category's entry scan finds the entry and tallies its reading.

## `private static async Task TLiveryLanguageSettle(LEngine engine, long entryId)`

Polls `TEngineFanqieCheck` until the fetch settles, failing after `TLiveryLanguagePatience`.

## `private static LEntryDraft TLiveryLanguageCreate(string headword, string language)`

Builds a draft with one meaning card in the given language.
