# CWing.cs

## `public sealed class CWing`

One side of the duplex panel: the entry vista it holds and the reading display under it.
It finds the rows, takes the query, order and language sieve, and loads the chosen entry.
The chosen entry is saved to the workspace state under the side, so the next run reopens it.
The driver owns the match list, the keys, the focus and the lectern that paints the display.

## `internal CWing(CAtelier atelier, CEnvoy envoy)`

Takes the atelier the side reads through and the envoy that reports a failed load.
The reading display is built here, since a side has no editor to own it.

## `public static CWing CWingCreate(CAtelier atelier, CEnvoy envoy)`

Builds one side over the atelier.
Building it is no user action, so it is no gate on the atelier.

## `public event Action<CBulletin>? CWingChanged;`

Raised on each vista, entry, reflex and settings announcement, so the driver re-lists the matches.
Order, filter or query moved, or a stored entry, a reflex fill or a flipped setting changed a listed row.

## `public event Action? CWingLoaded;`

Raised once an entry is loaded, so the driver re-lists its matches and paints the loaded entry.
A load that failed raises nothing, since the side stands where it stood.

## `public LDisplay CWingDisplay { get; }`

The reading display the driver's lectern paints.

## `public bool CWingFiltered`

Whether the vista hides any language, which the side shows as the mark on the sieve button.

## `public bool CWingQueried`

Whether the vista holds a query, so the side shows the list and its empty notice only then.

## `public CCatalogOrder CWingOrder`

The ordering the side lists in, headword order before a vista arrives.

## `public CCatalogFilter CWingFilter`

The languages the side hides, none before a vista arrives.

## `public void CWingVistaRestore(string tab)`

Starts a fresh vista under the side's tab, with entries in headword order and nothing chosen.
The display reads the same vista, and the side hears its announcements through `CWingChanged`.
A workspace switch calls it again, so the side follows the new workspace.

## `public void CWingOrderSet(CCatalogOrder? order)`

Hands a chosen ordering to the vista, which saves it under the side's tab and announces it.
A sender that is no order row hands null, which keeps the ordering it has.

## `public void CWingFilterSet(CCatalogFilter filter)`

Hands the ticked languages to the vista, which saves and announces them.

## `public void CWingEntrySelect(long? id)`

Moves the vista's choice without loading, for the keys that walk the list.

## `public void CWingEntryRestore(long? id)`

Loads the entry the side last showed, and a null id loads nothing.
The restore saves nothing, since the workspace state already names that entry.

## `public void CWingEntryOpen(long id)`

Loads the entry picked from the list and saves the side's standing.

## `public IReadOnlyList<CVistaRow> CWingRowsRead()`

The rows the engine returns for the vista, crossing as shapes so the side names no engine row.

## `private void LWingEntryLoad(long id)`

Loads one entry onto this side.
An entry that is gone leaves the side empty rather than showing what it was.
A load that failed leaves the side where it stood and reports the failure through the envoy.
The display loads and chooses the entry, so the side never holds the draft.
