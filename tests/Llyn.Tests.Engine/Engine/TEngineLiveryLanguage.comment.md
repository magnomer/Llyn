# TEngineLiveryLanguage.cs
Hash: `93be9c93139a750d`

## `public sealed class TEngineLiveryLanguage`

Calls the language read of `LEngineLiveryRead` through the `TLiveryRead` relay, one fact per part.
The `LiveryFormat` facts write those reads as notes through the `TLiveryFormat` relay.

## `private const string TLiveryLanguagePack`

The pack the fetched and respelled series facts load, naming one fanqie book and a series rule.

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
Reads that language and finds no series and no categories.

## `public void LiveryFormat_SeriesNote_LinksTheStoredEntryAndLeavesAnUnstoredCharacterPlain()`

Saves one entry and builds a series holding its character and one unstored character.
Writes the series note through `TLiveryFormat` and finds the stored character as an HTML anchor.
The unstored character stays plain, and neither shows Markdown link syntax.

## `public void LiveryFormat_SeriesNote_FoldsEachMemberReflexRowsByItsOpenedStateAndLeavesABareMemberAlone()`

Builds a series page with an opened stored member, a folded member and a bare member.
The opened member carries two readings and one Korean reflex, and the folded member one Korean reflex.
The stored entry carries the same main Korean reflex, so its entry note supplies the reference reflex table.
The series table must equal that entry table, header row and `llyn-main` cell alike.
The opened member is an open `llyn-card` details block, its anchored chip and escaped reading in the summary.
No summary holds Markdown link syntax.
The folded member is a closed `llyn-card` details block, its plain chip in the summary.
Each block holds the member's reflex table after its summary.
The bare member stays a `llyn-card` div holding only its plain chip.
No `llyn-more` block and no `Xiesheng.ReadingsShow` label remain.
Each member character appears exactly once, and no Markdown entry list repeats a member.

## `public void LiveryFormat_SeriesNoteRespelled_PrintsEachMemberRowAsTheEntryNotePrintsIt()`

Saves one entry whose main reflex is in a phonemic pack that respells, then stores its series.
Turns respelling on and writes the entry note and the series note from engine reads.
The entry row is picked by its language label, the one line it alone starts with.
It shows the slashed respelling, romanization, gloss and note, and no raw IPA.
The romanization is plain ASCII, since the escaper writes accented Latin letters as numeric entities.
The series note must hold that same row.

## `public void LiveryFormat_RimeNote_PrintsSectionsInPageOrderWithReadingsAndTallies()`

Reads the placed character's rime category and swaps in two sections, the second holding a tally row.
Checks section order, each line's reading and label, the rounded mark and the tally in the second section.

## `public void LiveryFormat_ToneNote_PrintsTheToneLabelAndLinksThePlacedEntry()`

Reads the placed character's tone category and writes its note with a `Display.FanqieTone` pattern.
Checks the heading shows the formatted tone and the entry list links the placed entry.

## `private static LEntry TLiveryLanguagePlace(LEngine engine, TWorkspace workspace)`

Saves one entry in the book language with a Korean reflex, stores its fanqie row and applies the tables.
The anchor sits on a reflex row, so without the reflex no row is anchored and every category reads empty.
Anchors that reflex to the row so each category's entry scan finds the entry and tallies its reading.

## `private static async Task TLiveryLanguageSettle(LEngine engine, long entryId)`

Polls `TEngineFanqieCheck` until the fetch settles, failing after `TLiveryLanguagePatience`.

## `private static LEntryDraft TLiveryLanguageCreate(string headword, string language)`

Builds a draft with one meaning card in the given language.
