# TXiesheng.cs
Hash: `83ab5684508b32c9`

## `public sealed class TXiesheng`

Covers the xiesheng panel's gates end to end on a real workspace.
The series are fetched from fake pages, so nothing here reaches the web.
`TEntryList` covers the entry list's load, export, scribe mode and close over this panel.

The bundled series pack shows the tab, and with nothing fetched the panel lists nothing under the bare keys.
Opening a fetched series by key lists it chosen with its count, and shows its page and entry.
The page carries the headword and glyph fonts of the series language, and the blank page carries none.
A blank or unknown key opens nothing.
An arrival empties the series search before the driver hears of it.
Choosing the chosen series unchooses it, choosing it again brings it back, and a workspace change clears it.
Choosing a series over an unsaved draft asks the leave question first.
A kept draft leaves the series and the draft as they were, and a discarded one closes the editor.
A glyph opens in the library tab only while a series page shows, in the language of that page.
Unmatched queries read the unmatched keys, and a narrowed column unchooses its series.
A null order keeps the chosen one.

## `internal const string TXieshengPack`

A pack that declares one fanqie source and one series source, each fetched from a fake page.

## `public void XieshengGroveRead_NothingFetched_ListsNothingUnderTheBareKeys()`

With nothing fetched, the column and the entry list read empty under their bare keys.
The bundled series pack still shows the tab, the page is blank, and the reader shows an entry.

## `public async Task XieshengStemOpen_FetchedSeries_ChoosesItAndListsItsEntry()`

Opening a fetched series by key lists it chosen, with its count of one.
The change is raised once, and the reader shows the series page instead of an entry.
The page names the language and the key, and its members carry only the character.
The entry list holds the series entry.

## `public async Task XieshengStemRead_ShownSeries_CarriesTheFontsOfTheSeriesLanguage()`

The blank page read before any series opens carries no font for the headword or the glyph.
An open series page carries the fonts the settings port reads for its language, headword and glyph roles.

## `public async Task XieshengStemOpen_BlankOrUnknownKey_ChoosesNothing()`

A missing, empty or unknown key opens no series, so the column keeps none chosen.
The reader keeps showing an entry.

## `public async Task XieshengStemSelect_ChosenTwiceThenWorkspaceChanged_TogglesAndClears()`

Choosing the chosen series unchooses it and hides its page.
Choosing it again brings it back with its entry listed.
A workspace notice then unchooses it and empties the entry list.

## `public async Task XieshengStemSelect_UnsavedDraftKept_ChangesNothing()`

A fresh draft with a typed headword puts the leave question once.
The user keeps it, so the series stays chosen and the editor keeps the draft.
No change is raised.

## `public async Task XieshengStemSelect_UnsavedDraftDiscarded_TogglesAndCloses()`

The same question is put once, and the user discards the draft.
The series is unchosen, the editor closes, and the change is raised once.

## `public async Task XieshengGlyphSelect_StemShown_OpensTheGlyphEntryInTheSeriesLanguage()`

A glyph opens nothing while no series page shows.
With a page shown, a blank glyph still opens nothing and a character opens its entry in the library tab.
The entry is the one the engine resolves for that character in the series language.

## `public async Task XieshengApertureQuerySet_UnmatchedQuery_ReadsTheUnmatchedKeys()`

An unmatched entry query empties the entry list under its unmatched key.
An unmatched series query empties the column under its unmatched key and unchooses the series.

## `public async Task XieshengStemOpen_QueriedSeries_EmptiesTheQueryBeforeTheOpening()`

The series search is already empty when the opened notice reaches the driver.
The series is then chosen and its page shows.

## `public void XieshengApertureOrderSet_NullAfterAnOrder_KeepsTheChosenOne()`

The column starts ordered by name.
A null order after a chosen one leaves the chosen order in place.

## `internal static CXiesheng TXieshengPrepare(CAtelier atelier)`

Builds the xiesheng panel over a fake envoy that answers no.

## `internal static CXiesheng TXieshengPrepare(CAtelier atelier, CEnvoy envoy)`

Builds the xiesheng panel over the given envoy, so a test can answer the questions the panel asks.
`TEntryList` builds its export panel through it.

## `internal static LEngine TXieshengEngineStart(TWorkspace workspace)`

Starts the engine with fake pages for the rime book and the series module of one character.

## `internal static async Task<LEntry> TXieshengStemSave(LEngine engine, string language)`

Stores one entry in the series pack's language and waits for its series fetch to settle.
