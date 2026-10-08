# TEntryList.cs
Hash: `3ed9071e02cf0aad`

## `public sealed class TEntryList`

Covers the entry list shared by the xiesheng and yunjing panels, on a real workspace.
It builds its panels through `TXiesheng` and `TYunjing`, so their fixtures place the entries.
Its flag-fill load answers the series entries and the catalog languages once the fill has run.
Export writes nothing until an entry is chosen, and the chosen entry leaves the page for the reader.
An entry opened in scribe mode shows in the editor, and closing the panel empties it.
The atelier's close lets each list's draft go and stops the recording.

## `public async Task EntryListLoad_FetchedSeries_AnswersItsEntryAfterTheFill()`

An opened series answers its one entry and the catalog's languages once the flag fill has run.
The test hands a store that keeps nothing, so only the sheet is compared.

## `public async Task EntryListExport_ChosenEntry_WritesItAndNothingBefore()`

Export writes no file while no entry is chosen.
Choosing an entry leaves the series page for the reader.
The file then holds the entry, and the offered name carries its headword.

## `public async Task EntryListPanelRowOpen_ScribeOn_OpensTheEntryInTheEditorUntilClosed()`

A row opened with scribe mode on shows in the editor and puts its entry on the desk.
Closing the entry hides the editor and empties the desk.

## `public async Task EntryListClose_KindredExitGate_ClosesTheEditorAndStopsTheRecording()`

The atelier's close lets the xiesheng list's held draft go and stops the recording once.

## `public void EntryListClose_XiaoyunExitGate_ClosesTheEditorAndStopsTheRecording()`

The atelier's close lets the yunjing list's held draft go and stops the recording once.
