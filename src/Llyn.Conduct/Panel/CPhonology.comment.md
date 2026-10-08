# CPhonology.cs
Hash: `24c97fa1cb0defa4`

## `public sealed class CPhonology`

The phonology panel's session: every entry with its primary pronunciation, the panel and the entry editor.
It finds the rows, while the panel's aperture takes the query, order and language filter.
Its panel loads, edits and deletes the chosen entry, and its own editor edits it.
It restores its vista itself, so no driver holds a port or a vista.

## `private CPhonology(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Takes the atelier's ports and builds the panel's own entry editor.
The panel asks the editor's desk before it leaves an entry, and finishes through the editor.
A cleared panel empties the editor, and an edited row opens in it.
It registers its vista restore and its close with `CWorkspace`, as every area does.
It restores its vistas last, so a built area already stands on started vistas.

## `public static CPhonology CPhonologyCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the panel over the atelier, so the driver hands it no port.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows.
`marshal` carries every engine notice onto the driver's thread, so the driver holds no observer.

## `public event Action? CPhonologyWorkspaceChanged;`

The workspace changed under the panel, after the panel let its chosen entry go.
The driver answers it by drawing the flags of the languages again.

## `public CEditor CPhonologyEditor { get; }`

The entry editor the panel opens its chosen entry in.
The driver wraps it for the editor view and the lectern.

## `internal void LPhonologyVistaRestore()`

Starts the phonology vista, ordered by headword until the user picks another order.
The aperture carries the former vista's search text into the fresh one, so a workspace switch keeps it.
The vista then goes to the panel and the editor, and the panel's observers attach to it.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `private void LPhonologyObserverAttach()`

Attaches the panel's answers to the fresh vista, each carried through the marshal.
A Vista, Reflex or Settings notice raises the rows again.
An Entry notice goes to the panel, and a notice on the chosen entry reads its draft again.
A Workspace notice goes to `LPhonologyWorkspaceResonate`.

## `private void LPhonologyWorkspaceResonate()`

Lets the chosen entry go, then tells the driver through `CPhonologyWorkspaceChanged`.

## `private void LPhonologyClose()`

The panel's part of the atelier's close.
The editor lets its draft go, then the display stops its playback.
`CAtelierClose` runs it through the closure the constructor registers.

## `public static IReadOnlyList<CCatalogOrder> CPhonologyOrderRead()`

The orders the sequence menu offers, in the order it lists them.
It needs no session, so the driver builds the menu once.

## `public IReadOnlyList<CCatalogPronunciation> CPhonologyRowsRead()`

The rows the engine returns for the vista, already filtered, sorted, twinned and marked.
Each maps plainly to its shape: the entry as a `CVistaRow` and the sound beside it.
The engine fills every epithet, so the map copies it as it stands.
It answers nothing before the vista is restored.
The count goes to the aperture's `CApertureEmpty`, so no driver counts rows.
A failed read shows `Sound.LoadFailed` through the envoy and answers no rows, so the count reads zero.
The load task runs this read after the flag fill, so a throw here would fault the driver.

## `public Task<CEnsignSheet<IReadOnlyList<CCatalogPronunciation>>> CPhonologyRowsLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Runs the flag fill into the driver's `store`, then answers `CPhonologyRowsRead` beside the loaded languages.
The shared rule `CCatalog.LCatalogEnsignLoad` orders the two, so the driver makes one request.
A failed flag fill shows `Sound.LoadFailed` and still answers the rows with no languages.
The driver awaits it from an event handler, where a fault would end the app.

## `internal string LPhonologyFileRead()`

The file name an export of the chosen entry is offered under, read from the phonology vista.

## `public Task CPhonologyPortraitPrint()`

Prints the chosen entry as the engine portrays it.
The reader is asked for the printer through the panel's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the panel's envoy.
`CPhonologyPortraitExport` exports it to a file the same way.

## `public Task CPhonologyPortraitExport()`

The file is offered under the entry's headword, as `LVistaPort.LEngineFileRead` words it.
`CPortrait` reads that name only once the export starts, and a failed read shows `Export.NameFailed`.
The reader is asked for the file and format through the panel's envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the panel's envoy.
