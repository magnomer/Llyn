# TYunjing.cs
Hash: `7c8e32081613fc0b`

## `public sealed class TYunjing`

Covers the yunjing panel's gates end to end on a real workspace.
The cells are placed straight into the store, so nothing here reaches the web.
`TYunjingVista` covers the vista restore and its observers, and `TEntryList` covers the list's close.
`TYunjingDiwei` covers the shown page, and `TYunjingPortrait` covers export and print.
The flag-fill load answers the cell's entries and the catalog languages once the fill has run.

The bundled rime-book pack shows the tab, and with nothing placed the panel lists nothing under the bare keys.
With no cell chosen, the columns list the first language declaring a book, here the bundled one.
Opening a cell by key lists its language with the cell chosen, and shows its page and its entries.
An unknown key opens nothing and raises nothing.
An arrival empties both column searches before the driver hears of it.
Choosing the chosen rime again hides the page, and a click without an id or a side does nothing.
Choosing a cell over an unsaved draft asks the leave question first.
A kept draft leaves the cell and the draft as they were, and a discarded one closes the editor.
Unmatched queries read the unmatched keys, and a narrowed column unchooses its cell.
A null order keeps the chosen one in each column.
An entry opened in scribe mode shows in the editor, and closing the entry empties it.
The order menu offers three orderings.

## `internal const string TYunjingPack = """{ "language": "Fixture" }""";`

A pack that names one fixture language, in which the tests place their cells.

## `internal const string TYunjingBook = "Classical Chinese";`

The language the bundled rime-book pack declares, which the columns list while no cell is chosen.

## `public async Task YunjingEntryListLoad_FixtureCell_AnswersTheCellEntriesAfterTheFill()`

An opened cell answers its entry and the catalog's languages once the flag fill has run.
The test hands a store that keeps nothing, so only the sheet is compared.

## `public void YunjingShengmuRead_NothingPlaced_ListsNothingUnderTheBareKeys()`

With nothing placed, both columns and the entry list read empty under their bare keys.
The bundled rime-book pack still shows the tab, the page is blank, and the reader shows an entry.

## `public void YunjingShengmuRead_NoCellChosen_ListsTheFirstBookLanguage()`

With no cell chosen, the columns list the cells of the bundled book language and none is chosen.
The entry list stays empty, since it follows the chosen cell.

## `public void YunjingDiweiOpen_FixtureCell_ListsThatLanguageWithTheCellChosen()`

Opening an initial by key lists that language's cells with the opened one chosen.
The change is raised once, and the reader shows the cell page instead of an entry.
The page kind follows the initial column, and the entry list holds the cell's entry.

## `public void YunjingDiweiOpen_UnknownKey_ChoosesNothing()`

An initial the store never held opens nothing and raises no change.

## `public void YunjingDiweiSelect_ChosenRimeAgain_HidesThePage()`

A rime opened by key makes the page kind the rime one.
Choosing it again hides the page and empties the entry list.
A click without an id or without a side changes nothing.

## `public void YunjingDiweiSelect_UnsavedDraftKept_ChangesNothing()`

A fresh draft with a typed headword puts the leave question once.
The user keeps it, so the rime stays chosen and the editor keeps the draft.
No change is raised.

## `public void YunjingDiweiSelect_UnsavedDraftDiscarded_TogglesAndCloses()`

The same question is put once, and the user discards the draft.
The rime is unchosen, the editor closes, and the change is raised once.

## `public void YunjingApertureQuerySet_UnmatchedQuery_UnchoosesAndReadsTheUnmatchedKeys()`

An unmatched entry query empties the entry list under its unmatched key.
Unmatched queries on both columns empty them under their unmatched keys.
A narrowed initial column unchooses its cell, so the page hides.

## `public void YunjingDiweiOpen_QueriedColumns_EmptiesBothQueriesBeforeTheOpening()`

Both column searches are already empty when the opened notice reaches the driver.
The cell is then chosen and its page shows.

## `public void YunjingApertureOrderSet_NullAfterAnOrder_KeepsTheChosenOne()`

Both columns start ordered by name.
A null order after a chosen one leaves the chosen order in place in each column.

## `public void YunjingPanelRowOpen_ScribeOn_OpensTheEntryInTheEditorUntilClosed()`

A row opened with scribe mode on shows in the editor and puts its entry on the desk.
Closing the entry hides the editor and empties the desk.

## `public void YunjingOrderRead_Menu_OffersTheThreeOrderings()`

The order menu lists name, reverse and usage, in that order.

## `internal static CYunjing TYunjingPrepare(CAtelier atelier)`

Builds the yunjing panel over a fake envoy that answers no.

## `internal static CYunjing TYunjingPrepare(CAtelier atelier, CEnvoy envoy)`

Builds the yunjing panel over the given envoy, so a test can answer the questions the panel asks.
