# CTaxonomy.cs
Hash: `25df419e063a091b`

## `public sealed class CTaxonomy`

The taxonomy panel's session: the Tag list, the entry editor, and the entry list it hands to `CMembership`.
The tag vista and the membership vista hold the panel's state, and the drivers only follow.
Every gate holds only the interaction and reaches the engine through a vista, the panel or the editor's desk.
It restores both vistas itself, so no driver holds a port or a vista.

## `private CTaxonomy(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the aperture over the tag vista first, under `Tag.LoadFailed`.
Then it builds the entry editor and the entry list over the atelier's ports.
The entry list's panel asks the editor's desk before it leaves an entry, and it finishes through the editor.
A cleared panel cancels the editor's draft, and an edited row opens the editor on it.
The editor's display follows the panel, so each draft it loads opens the reading view.
It registers its close with the workspace beside its draft finish and its vista restore.
It restores its vistas last, so a built area already stands on started vistas.

## `public static CTaxonomy CTaxonomyCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the taxonomy over the atelier and the panel's own entry editor.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.
`marshal` carries every engine notice onto the driver's thread, and the driver hands it once.

## `public CAperture CTaxonomyAperture { get; }`

The list slot of the Tag list, which holds the tag vista and raises the Tag rows.
Its query, ordering and filter are the Tag list's, and a driver sets them through it.
Its chosen Tag is the station the voyage records.
The entry rows follow a Tag rows read, so the Tag list is always read first.

## `public CEditor CTaxonomyEditor { get; }`

The entry editor on the membership side, which the driver wraps for its editor page.

## `public CMembership CTaxonomyMembership { get; }`

The entry list beside the Tag list, with its own panel, search and rows read.

## `public event Action? CTaxonomyTagOpened;`

Raised when an arrival or a coinage opens a Tag, so the driver shows both searches empty.
The aperture's rows notice follows it, so the lists are read after the fields are emptied.

## `public event Action? CTaxonomyWorkspaceChanged;`

Raised last when the workspace moved, after the entry is closed, the Tag let go and the rows raised.
The driver loads the new workspace's flags on it.

## `public void CTaxonomyTagToggle(long id)`

A click on a row: records the station it leaves, then toggles the Tag.
It raises the rows, since choosing a Tag announces nothing in the engine.

## `internal void LTaxonomyTagOpen(long id)`

The navigation's arrival: chooses the Tag and empties both searches, then raises `CTaxonomyTagOpened`.
Emptying the searches is the arrival's own, so no driver has to echo it back through the query gates.
It raises the rows last, since the Tag may be new to the list.

## `private bool LTaxonomyCoinageAllowed`

Whether New should name a new Tag rather than start an entry.
That holds while no Tag is chosen and no entry is shown.
Only the fresh gate reads it, after the leave question settled the shown entry.

## `internal void LTaxonomyVistaRestore()`

Starts the tag vista and the membership vista for this tab.
Each aperture carries its held query into the fresh vista, so a switched workspace keeps both searches.
The tag vista is the entry list's roll, and the membership vista goes to the entry list and the editor.
The observers are attached to the fresh vistas last.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `private void LTaxonomyObserverAttach()`

The taxonomy's subject plan, attached to each fresh pair of vistas.
Every notice runs through the marshal, so each answer lands on the driver's thread.
Each answer attaches to the tag vista through the aperture.
A vista, reflex, settings or entry notice on the tag vista raises the aperture's rows.
A reflex fill or a flipped setting rewrites the epithet beside a headword.
A workspace notice and a Tag notice each have their own answer.
The entry list attaches its own observers in between, in the order the driver once used.
A draft edit or a fetched frequency, paradigm, script or fanqie row changes no listed row, and is not attached.

## `private void LTaxonomyWorkspaceResonate()`

Puts the panel back on the workspace open now.
It closes the entry, lets go of the Tag, raises the rows, then raises `CTaxonomyWorkspaceChanged`.
A different workspace has its own Tags, so the Tag this panel stood on may not exist there.

## `private void LTaxonomyTagResonate()`

A Tag notice carries a Tag id, not an entry id, so it never selects a row.
It raises the rows, then rereads the shown entry, whose chips may carry the renamed Tag.

## `public IReadOnlyList<CCatalogTag> CTaxonomyRowsRead()`

The Tag rows the engine lists, chosen mark included, mapped through the card's one Tag map.
It answers nothing before the vistas are restored.
The engine drops a chosen Tag the rows no longer hold, so the entries fall back to every entry.
A failed read shows `Tag.LoadFailed` through the envoy and answers no rows.
A read that succeeds raises the entry rows, since the entries hang on the chosen Tag.
A failed read raises nothing more, so the user sees one notice, as before.

## `public Task<CEnsignSheet<IReadOnlyList<CCatalogTag>>> CTaxonomyRowsLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Runs the flag fill into the driver's `store`, then answers `CTaxonomyRowsRead` beside the loaded languages.
The shared rule `CCatalog.LCatalogEnsignLoad` orders the two, so the driver makes one request.
A failed flag fill shows `Tag.LoadFailed` and still answers the rows with no languages.
The driver awaits it from an event handler, where a fault would end the app.

## `private void LTaxonomyTagCreate(string name)`

Makes the Tag from the raw wording and opens it as an arrival does.
The clerk below refuses a blank wording.
The panel is cleared first, so an unsaved fresh entry the leave check already settled does not linger.
A refused Tag is shown through `CEnvoy` as `Tag.CreateFailed`, and nothing opens.

## `public void CTaxonomyEntryCreate()`

The New button: the one gate for a fresh Tag or a fresh entry.
It asks the leave question first, and a user who stays gets nothing new.
While a Tag may be coined, it asks the wording through `CEnvoy` under `Coinage.Tag` and makes the Tag.
A retreat from that question makes nothing.
Otherwise it opens a fresh entry in the editor, in edit mode.
The chosen Tag goes down with the start, so the engine puts it on the first card in one call.
With no Tag chosen the fresh entry starts blank.

## `private void LTaxonomyClose()`

The taxonomy's part of the window's exit gate `CAtelier.CAtelierClose`, registered with the workspace at build.
It closes the entry editor and cancels its display's playback.
