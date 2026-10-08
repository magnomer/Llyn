# CCohort.cs
Hash: `e54fe06dab57a550`

## `public sealed class CCohort`

The tenor's entry list: the entries carrying the chosen Register, and the one the reader stands on.
It is shaped like `CQuotation`, and the tenor owns it as `CCorpus` owns its quotation.

## `internal CCohort(LVistaPort vistas, LPortraitPort portraits, LSettingsPort settings, CEnvoy envoy, Func<bool> changeSeam, Func<bool, bool> finishSeam, Func<bool> shownSeam)`

Builds the panel over the cohort vista, which reports a failed load as `Register.LoadFailed`.
Its empty list reads `Register.Unmatched` under a search and `Register.Vacant` without one.
The panel asks the editor's desk before it leaves an entry, and it finishes through the editor.

## `public CPanel CCohortPanel { get; }`

The shared panel state over the cohort vista: the chosen entry, the scribe mode, the bin and the leave guard.

## `internal void LCohortVistaRestore(LVista roll, LVista vista)`

Takes the fresh register vista as its roll and hands the fresh cohort vista to the panel's aperture.
The aperture carries the held query into it, so a switched workspace keeps the search.

## `internal void LCohortObserverAttach(Action<Action> marshal)`

Attaches the entry list's observers to its fresh vista, each through the marshal.
An entry notice goes to the panel, whose chosen row it may name.
A vista notice raises the entry rows alone, since only the entry search moved.
A notice on the chosen entry rereads its draft.

## `public IReadOnlyList<CVistaRow> CCohortRowsRead()`

The entries under the chosen Register, as the engine narrows them.
A failed read shows `Register.LoadFailed` through the envoy and answers no rows.

## `internal string LCohortFileRead()`

The file name the export offers for the shown entry, as `LVistaPort.LEngineFileRead` words it.

## `public Task CCohortPortraitPrint()`

Prints the chosen entry, and does nothing while none is chosen or the panel is editing.
`CCohortPortraitExport` exports it under the same condition.
The reader is asked for the printer through the envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the envoy.

## `public Task CCohortPortraitExport()`

The file is offered under the entry's headword, as `LVistaPort.LEngineFileRead` words it.
`CPortrait` reads that name only once the export starts, and a failed read shows `Export.NameFailed`.
The reader is asked for the file and format through the envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the envoy.
