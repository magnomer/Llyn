# CWing.cs
Hash: `d8282b6d0dedf72d`

## `public sealed class CWing`

One side of the duplex panel: the entry vista it holds and the reading display under it.
It finds the rows, takes the query, order and language sieve, and loads the chosen entry.
It moves the choice through the listed rows for the keys that walk the list.
The chosen entry is saved to the workspace state under the side, so the next run reopens it.
It restores itself: its vista on every workspace change and its entry on every open.
The driver owns the match list, the focus and the lectern that paints the display.

## `private readonly bool _cWingLeft;`

Whether this is the left side, which names its vista's tab and its slot in the workspace state.

## `private CWing(CAtelier atelier, CEnvoy envoy, bool left)`

Takes the atelier the side reads through, the envoy that reports a failed load, and which place it is.
The reading display is built here, since a side has no editor to own it.
It hands the display the atelier's repaint memory, so both sides and every editor share it.
It opens what it is asked to through the atelier's navigation.
It starts its first vista at once, so the driver's first paint finds one.
It registers its vista restore and its close with the workspace, as every area does.
It hears the workspace state itself, so no driver hands an entry id down.

## `public static CWing CWingCreate(CAtelier atelier, CEnvoy envoy, bool left)`

Builds one side over the atelier, the left place when `left` is true.
Building it is no user action, so it is no gate on the atelier.

## `public event Action<CBulletin>? CWingChanged;`

Raised on each vista, entry, reflex and settings announcement, so the driver re-lists the matches.
Order, filter or query moved, or a stored entry, a reflex fill or a flipped setting changed a listed row.

## `public event Action? CWingLoaded;`

Raised once an entry is loaded, so the driver re-lists its matches and paints the loaded entry.
A load that failed raises nothing, since the side stands where it stood.

## `public event Action? CWingRowsChanged;`

Raised when a key moved the choice, so the driver re-marks the rows in place.
A selection raises no engine announcement, so the side raises its own.

## `public LDisplay CWingDisplay { get; }`

The reading display the driver's lectern paints.

## `public bool CWingFiltered`

Whether the vista hides any language, which the side shows as the mark on the sieve button.

## `public bool CWingQueried`

Whether the vista holds a query, so the side shows the list and its empty notice only then.

## `public bool CWingEmpty`

Whether the side shows its empty notice.
A query is held and the last rows read found nothing.
A list not yet asked stays blank, so an unqueried side is never empty.

## `public CCatalogOrder CWingOrder`

The ordering the side lists in, headword order before a vista arrives.

## `public CCatalogFilter CWingFilter`

The languages the side hides, none before a vista arrives.

## `private void LWingVistaRestore()`

Starts a fresh vista under the side's tab, with entries in headword order and nothing chosen.
It forgets the rows last read, so the keys move nothing until the new rows are read.
The display reads the same vista, and the side hears its announcements through `CWingChanged`.
The workspace runs it on every change, so the side follows the new workspace.
The fresh vista starts with nothing typed and nothing chosen, so neither needs setting.

## `public void CWingQuerySet(string query)`

Hands the search text to the vista, which announces only a changed text.
A workspace change starts a fresh vista with nothing typed, so the query is not carried over.

## `public void CWingOrderSet(CCatalogOrder? order)`

Hands a chosen ordering to the vista, which saves it under the side's tab and announces it.
A sender that is no order row hands null, which keeps the ordering it has.

## `public void CWingFilterSet(CCatalogFilter filter)`

Hands the ticked languages to the vista, which saves and announces them.

## `public long? CWingRowMove(bool down)`

Moves the vista's choice one listed row down or up, without loading or saving.
The move walks the rows last read, which are the rows the driver shows.
It stops at either end, and no chosen row counts as the place before the first.
So Down and Up both land on the first row when nothing is chosen.
It raises `CWingRowsChanged` and answers the chosen entry, which the driver scrolls into view.
An empty list answers null and raises nothing, so the key stays with the field.

## `private void LWingEntryRestore(CWorkspaceState state)`

Answers every workspace open with the entry `state` names for this side.
The shown entry closes first, so a failed load never leaves another workspace's entry standing.
An empty slot loads nothing.
The restore saves nothing, since the workspace state already names that entry.

## `public bool CWingEntryOpen(long? id)`

Loads the entry picked from the list and saves the side's standing.
A click hands its row's id and Enter the chosen one, so no control decides the request.
No row picks nothing and answers false, so the key stays with the field and the list stays open.

## `public IReadOnlyList<CVistaRow> CWingRowsRead()`

The rows the engine returns for the vista, crossing as shapes so the side names no engine row.
The side holds them, so the keys and the empty notice follow what the driver shows.

## `private void LWingClose()`

Stops the display's playback when the atelier closes, registered with the workspace's closures.

## `private void LWingEntryLoad(long id)`

Loads one entry onto this side.
An entry that is gone leaves the side empty rather than showing what it was.
A load that failed leaves the side where it stood and reports the failure through the envoy.
The display loads and chooses the entry, so the side never holds the draft.
