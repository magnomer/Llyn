# CCohort.cs

## `public sealed class CCohort`

The tenor's entry list: the entries carrying the chosen Register, and the one the reader stands on.
It is shaped like `CQuotation`, and the tenor owns it as `CCorpus` owns its quotation.

## `internal CCohort(LEntryPort entries, LPortraitPort portraits, LSettingsPort settings, CEnvoy envoy, Func<bool> changeSeam, Func<bool, bool> finishSeam, Func<bool> shownSeam)`

Builds the panel over the cohort vista, which reports a failed load as `Register.LoadFailed`.
The panel asks the editor's desk before it leaves an entry, and it finishes through the editor.

## `public CPanel CCohortPanel { get; }`

The shared panel state over the cohort vista: the chosen entry, the scribe mode, the bin and the leave guard.

## `public string CCohortEmptyKey`

The localization key for the empty entry list.
The engine says whether the search holds text, so a search reads as unmatched and no search as vacant.

## `internal void LCohortVistaRestore(LVista roll, LVista vista)`

Takes the fresh register vista as its roll and the fresh cohort vista as its own.
The fresh vista takes the query its forerunner held, so a switched workspace keeps the search.

## `internal void LCohortObserverAttach(Action<Action> marshal)`

Attaches the entry list's observers to its fresh vista, each through the marshal.
An entry notice goes to the panel, whose chosen row it may name.
A vista notice raises the entry rows alone, since only the entry search moved.
A notice on the chosen entry rereads its draft.

## `public void CCohortQuerySet(string query)`

Narrows the entries of the chosen Register by the text typed in the entry search.

## `public IReadOnlyList<CVistaRow> CCohortRowsRead()`

The entries under the chosen Register, or every entry while none is chosen, as the engine narrows them.
A failed read shows `Register.LoadFailed` through the envoy and answers no rows.

## `public Task CCohortPortraitPrint()`

Prints the shown entry, and does nothing while none is shown outside edit mode.
`CCohortPortraitExport` exports it under the same condition.
The reader is asked for the printer through the envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the envoy.
The reader is asked for the file and format through the envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the envoy.
