# CPhonology.cs

## `public sealed class CPhonology`

The phonology panel's session: every entry with its primary pronunciation, the vista and the entry editor.
It finds the rows, and takes the query, order and language filter.
Its panel loads, edits and deletes the chosen entry, and its own editor edits it.
It restores its vista itself, so no driver holds a port or a vista.

## `private CPhonology(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)`

Takes the atelier's ports and builds the panel's own entry editor.
The panel asks the editor's desk before it leaves an entry, and finishes through the editor.
A cleared panel empties the editor, and an edited row opens in it.

## `public static CPhonology CPhonologyCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)`

Builds the panel over the atelier, so the driver hands it no port.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.

## `public CEditor CPhonologyEditor { get; }`

The entry editor the panel opens its chosen entry in.
The driver wraps it for the editor view and the lectern.

## `public bool CPhonologyEmpty`

Whether the last rows read found nothing, so the empty notice is a verdict and no driver counts rows.

## `public bool CPhonologyFiltered`

Whether the language filter hides any language, as the vista answers it.

## `public void CPhonologyVistaRestore()`

Starts the phonology vista, ordered by headword until the user picks another order.
The vista then goes to the panel and the editor.

## `public IReadOnlyList<CCatalogPronunciation> CPhonologyRowsRead()`

The rows the engine returns for the vista, already filtered, sorted, twinned and marked.
Each maps plainly to its shape: the entry as a `CVistaRow` and the sound beside it.
The engine fills every epithet, so the map copies it as it stands.
It answers nothing before the vista is restored.

## `public void CPhonologyOrderSet(CCatalogOrder? order)`

Orders the rows as the user chose.
A null order keeps the current one, which the vista decides.

## `public Task CPhonologyPortraitPrint(CPortraitLabel label, CPressTicket ticket)`

Prints the chosen entry as the engine portrays it, with the labels the driver localized.
`CPhonologyPortraitExport` exports it to a file the same way.
