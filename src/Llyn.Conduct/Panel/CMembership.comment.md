# CMembership.cs
Hash: `64028e7aaa6989ee`

## `public sealed class CMembership`

The taxonomy's entry list: the entries carrying the chosen Tag, and the one the reader stands on.
It is shaped like `CQuotation`, and the taxonomy owns it as `CCorpus` owns its quotation.

## `internal CMembership(LEntryPort entries, LPortraitPort portraits, LSettingsPort settings, CEnvoy envoy, Func<bool> changeSeam, Func<bool, bool> finishSeam, Func<bool> shownSeam)`

Builds the panel over the membership vista, which reports a failed load as `Tag.LoadFailed`.
The panel asks the editor's desk before it leaves an entry, and it finishes through the editor.

## `public CPanel CMembershipPanel { get; }`

The shared panel state over the membership vista: the chosen entry, the scribe mode, the bin and the leave guard.

## `public string CMembershipEmptyKey`

The localization key for the empty entry list.
The engine says whether the search holds text, so a search reads as unmatched and no search as vacant.

## `internal void LMembershipVistaRestore(LVista roll, LVista vista)`

Takes the fresh tag vista as its roll and the fresh membership vista as its own.
The fresh vista takes the query its forerunner held, so a switched workspace keeps the search.

## `internal void LMembershipObserverAttach(Action<Action> marshal)`

Attaches the entry list's observers to its fresh vista, each through the marshal.
An entry notice goes to the panel, whose chosen row it may name.
A vista notice raises the entry rows alone, since only the entry search moved.
A notice on the chosen entry rereads its draft.

## `public void CMembershipQuerySet(string query)`

Narrows the entries of the chosen Tag by the text typed in the entry search.

## `public IReadOnlyList<CVistaRow> CMembershipRowsRead()`

The entries under the chosen Tag, or every entry while none is chosen, as the engine narrows them.
A failed read shows `Tag.LoadFailed` through the envoy and answers no rows.

## `public Task CMembershipPortraitPrint()`

Prints the shown entry, and does nothing while none is shown outside edit mode.
`CMembershipPortraitExport` exports it under the same condition.
The reader is asked for the printer through the envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the envoy.

## `public Task CMembershipPortraitExport()`

The file is offered under the entry's headword, as `LVista.LVistaFileRead` words it.
`CPortrait` reads that name only once the export starts, and a failed read shows `Export.NameFailed`.
The reader is asked for the file and format through the envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the envoy.
