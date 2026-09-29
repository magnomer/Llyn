# CShelf.cs

## `public sealed class CShelf`

The sources panel's session: the Source list, the entries citing the chosen Source, the source editor and the entry editor.
The two vistas' chosen rows and editing flags are the panel's mode, and the drivers only follow.
Every gate holds only the interaction and reaches the engine through a panel, a desk or the editor.
It asks the user and reports failures through the envoy, never through a seam a driver hands up.
It restores both vistas itself and chooses which side prints, exports, saves or deletes.

## `private CShelf(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the entry editor, the source editor, the Source panel and the entry list over the atelier's ports.
The Source panel asks the source desk before it leaves a Source, and both panels finish through `LShelfDraftFinish`.
A cleared Source panel cancels the source draft, and an edited row opens the source editor on it.
A loaded Source draft is shaped into its colophon through `COeuvre`'s map, with no second panel.
A stored Source is shown again outside the scribe.
Either desk's state change is raised as `CShelfChanged`, so a driver repaints the mode.
The Source panel's row notices reach the entry list, whose rows follow the chosen Source.
The source editor hears its desk notices through `marshal`.
The shelf's close joins the workspace's closures, so it runs when the atelier closes.

## `public static CShelf CShelfCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the shelf over the atelier and the panel's own entry editor.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.
`marshal` hands an engine notice onto the driver's thread.

## `public event Action? CShelfChanged`

Raised when the source desk or the entry editor changes state, so a driver repaints the mode.

## `public event Action<CColophon>? CShelfColophonChanged`

Raised with the colophon of each Source draft the Source panel loads.

## `public CEditor CShelfEditor { get; }`

The entry editor of the entry side, which the driver wraps for its editor page.

## `public CImprint CShelfImprint { get; }`

The source editor, whose desk holds the Source draft while the Source panel edits.

## `public bool CShelfEditorShown`

True while the entry side is in front and in edit mode, so the entry editor shows.
The other mode flags follow the same two facts: which side is in front and whether it edits.
`CShelfDisplayShown` holds for the entry side outside edit mode.
`CShelfImprintShown` and `CShelfColophonShown` hold for the Source side, in and outside edit mode.
`CShelfScribeChecked` holds while the side in front edits, and `CShelfViewerChecked` holds otherwise.
`CShelfModeEnabled` holds while either panel holds a row or edits.
`CShelfBinEnabled` holds only on the Source side, where it follows its panel.

## `public bool CShelfStoreEnabled`

On the entry side, whether the entry may be stored.
On the Source side, whether the Source draft holds a change.

## `public bool CShelfPressAllowed`

True while an entry or a Source is shown outside edit mode, so there is something to print.
`CShelfPortraitAllowed` holds only for a shown entry, since only an entry exports.

## `public bool CShelfEmpty`

True when the last rows read listed no Source.

## `public bool CShelfFiltered`

True while the Source vista hides some language, so a driver marks its filter.

## `private bool LShelfEntrySide`

The entry list holds a row or edits, so the entry side is in front.

## `public void CShelfVistaRestore()`

Starts the Source vista and the entry vista through the atelier and hands them to the panels and desks.
The entry list follows the Source vista as its parent, and the entry editor stands on the entry vista.

## `public IReadOnlyList<CCatalogReference> CShelfRowsRead()`

Reads the Sources the vista finds, mapped as the oeuvre maps its own, and counts them.
A shown Source the rows no longer choose is closed, so a stale selection never stays in front.

## `public void CShelfOrderSet(CCatalogOrder? order)`

Takes the chosen order, and the vista keeps its own order when none is chosen.

## `public static IReadOnlyList<CCatalogOrder> CShelfOrderRead()`

The orders the Source list offers, in the order a driver lists them.

## `public string CShelfTallyRead()`

The engine's worded tally of the chosen Source's citations.

## `internal bool LShelfChangeRead()`

Whether either panel edits a draft that holds unsaved changes.

## `internal bool LShelfLeaveConfirm()`

Asks through the envoy before unsaved work goes out of sight, and answers whether to go on.
A stored answer finishes the draft of the side in front, and a declined question stays.

## `public void CShelfReferenceClose()`

Closes the entry side and the Source side, as a workspace change does.

## `public void CShelfReferenceSelect(long? id)`

Opens the clicked Source after the leave question, keeping the edit mode of the side in front.
The navigation records the voyage station only once the leave is settled, so a kept leave records nothing.

## `internal void LShelfReferenceOpen(long id)`

The navigation's arrival: opens one Source without asking, since the navigation already asked.

## `public void CShelfEntrySelect(long? id)`

Opens the clicked entry after the leave question, keeping the edit mode, and drops the source draft.

## `public void CShelfEntryResonate()`

Answers the chosen entry's notice by refreshing the entry draft.
It falls back to the chosen Source once the entry side has closed.

## `public void CShelfReferenceCreate()`

The fresh button after the leave question.
With a row chosen, it opens a fresh entry already citing the chosen Source.
With nothing chosen, it opens a fresh Source in the source editor.

## `public void CShelfScribeToggle(bool editing)`

Switches the edit mode of the side in front.
A Source side leaving edit mode drops the source draft.
An entry side that closes falls back to the chosen Source.

## `public void CShelfReferenceDelete()`

Deletes the chosen Source through its panel, and does nothing on the entry side.

## `private void LShelfClose()`

Closes the byline when the atelier closes.

## `public (bool CShelfBackward, bool CShelfForward) CShelfChronicleRead()`

Whether the draft of the side in front can step back or forward.

## `internal bool LShelfDraftFinish(bool store)`

Finishes the draft of the side in front, storing or dropping it, and answers whether it ended.

## `public void CShelfDraftSave()`

Stores the draft of the side in front when it changed.

## `public void CShelfDraftUndo()`

Steps the draft of the side in front back.
`CShelfDraftRedo` steps it forward.

## `public Task CShelfPortraitPrint()`

Prints the shown entry, or else the shown Source with the Source realm's legend, and nothing while neither is shown.
The reader is asked for the printer through the panel's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the panel's envoy.

## `public Task CShelfPortraitExport()`

Exports the shown entry, and nothing while no entry is shown.
The reader is asked for the file and format through the panel's envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the panel's envoy.
