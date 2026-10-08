# CShelf.cs
Hash: `a7b11b8b857948e1`

## `public sealed class CShelf`

The sources panel's session over the Source list, the entries citing the chosen Source and two editors.
The two vistas' chosen rows and editing flags are the panel's mode, and the drivers only follow.
The routing between the two lists and its mode flags live in `CDiptych`, shared with the other two-list panels.
Every gate holds only the interaction and reaches the engine through a panel, a desk or the editor.
It asks the user and reports failures through the envoy, never through a seam a driver hands up.
It restores both vistas itself and chooses which side prints or exports.

## `private CShelf(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the entry editor, the source editor, the Source panel and the entry list over the atelier's ports.
The source editor also takes the atelier's repaint memory, for its byline's search.
The Source panel asks the source desk before it leaves a Source, and both panels finish through the session.
The session runs over the source desk and defers to the entry editor while the entry side is in front.
The diptych is built right after it, with the source editor's open and cancel as its seams.
Its create seam drops the source draft and opens a fresh entry citing the chosen Source.
A cleared Source panel cancels the source draft, and an edited row opens the source editor on it.
A loaded Source draft is shaped into its colophon through `COeuvre`'s map, with no second panel.
A stored Source is shown again outside the scribe.
The session's state change is raised as `CShelfChanged`, so a driver repaints the mode.
The Source panel's row notices reach the entry list, whose rows follow the chosen Source.
The source editor hears its desk notices through `marshal`.
The shelf keeps `marshal` for the observers it attaches at each vista restore.
The shelf's close joins the workspace's closures, so it runs when the atelier closes.
It closes the byline first and then the session's entry editor with its playback.
It restores its vistas last, so a built area already stands on started vistas.

## `public static CShelf CShelfCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the shelf over the atelier and the panel's own entry editor.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.
`marshal` hands an engine notice onto the driver's thread.

## `public event Action? CShelfChanged;`

Raised when the source desk or the entry editor changes state, so a driver repaints the mode.

## `public event Action<CColophon>? CShelfColophonChanged;`

Raised with the colophon of each Source draft the Source panel loads.

## `public CEditor CShelfEditor { get; }`

The entry editor of the entry side, which the driver wraps for its editor page.

## `public CPanel CShelfPanel { get; }`

The Source panel, whose rows are the Sources the vista finds.

## `public CFootnote CShelfFootnote { get; }`

The entry list, whose rows are the entries citing the chosen Source.

## `public CImprint CShelfImprint { get; }`

The source editor, whose desk holds the Source draft while the Source panel edits.

## `public CSession CShelfSession { get; }`

The draft session over the source desk, which the driver saves, undoes and redoes through.
It asks the leave question and finishes the draft of the side in front.

## `public CDiptych CShelfDiptych { get; }`

The routing between the Source panel and the entry list, with the side flags the drivers paint.
Its gates create and delete on whichever side is in front.
Its create seam opens a fresh entry for the chosen Source, else a fresh Source is started.

## `public bool CShelfStoreEnabled`

Whether the desk in front, the entry editor or the Source imprint, reports its draft storable.

## `public bool CShelfPressAllowed`

True while an entry or a Source is shown outside edit mode, so there is something to print.
`CShelfPortraitAllowed` holds only for a shown entry, since only an entry exports.

## `internal void LShelfVistaRestore()`

Starts the Source vista and the entry vista through the atelier and hands them to the panels and desks.
The entry list follows the Source vista as its parent, and the entry editor stands on the entry vista.
The Source panel's aperture carries the former vista's query into the fresh one, so the driver never re-sends its text.
Then the shelf attaches its own observers, and the entry list attaches its own with the shelf's answers handed in.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `private void LShelfObserverAttach()`

Attaches the Source list's subjects, each answered through `marshal`.
Vista, Author, Reference, Example, Reflex and Settings notices refill the Source rows.
An Author notice also rereads the source draft.
A Workspace notice closes both sides through the diptych's `LDiptychEntryClose`.

## `public CShelfRoll CShelfRollRead()`

Reads the Sources the vista finds, mapped as the oeuvre maps its own, into one answer.
A shown Source the rows no longer choose is closed, so a stale selection never stays in front.
The tally is read after that close, through the panel's own tally read and its failure report.
So the tally never counts a Source the panel just left.
A failed read shows `Source.LoadFailed` through the envoy and goes on with no Sources.
The roll is then the same empty roll a vista with no Sources gives, close and tally included.
The load task runs this read after the flag fill, so a throw here would fault the driver.

## `public Task<CEnsignSheet<CShelfRoll>> CShelfRollLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Runs the flag fill into the driver's `store`, then answers `CShelfRollRead` beside the loaded languages.
The shared rule `CCatalog.LCatalogEnsignLoad` orders the two, so the driver makes one request.
A failed flag fill shows `Source.LoadFailed` and still answers the rows with no languages.
The driver awaits it from an event handler, where a fault would end the app.

## `public static IReadOnlyList<CCatalogOrder> CShelfOrderRead()`

The orders the Source list offers, in the order a driver lists them.

## `public void CShelfReferenceSelect(long? id)`

Opens the clicked Source after the leave question, keeping the edit mode of the side in front.
The navigation records the voyage station only once the leave is settled, so a kept leave records nothing.

## `internal void LShelfReferenceOpen(long id)`

The navigation's arrival opens one Source without asking, since the navigation already asked.

## `public void CShelfEntrySelect(long? id)`

Opens the clicked entry after the leave question, keeping the edit mode, and drops the source draft.

## `private void LShelfEntryResonate()`

Answers the chosen entry's notice by refreshing the entry draft.
It falls back to the chosen Source once the entry side has closed.

## `public void CShelfScribeToggle(bool editing)`

Switches the edit mode of the side in front.
A Source side leaving edit mode drops the source draft.
An entry side that closes falls back to the chosen Source.

## `public Task CShelfPortraitPrint()`

Prints the shown entry, or else the shown Source with the Source realm's legend, and nothing while neither is shown.
The reader is asked for the printer through the panel's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the panel's envoy.

## `public Task CShelfPortraitExport()`

Exports the shown entry, and nothing while no entry is shown.
The reader is asked for the file and format through the panel's envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the panel's envoy.
