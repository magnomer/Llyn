# LWing.cs

## `public sealed class LWing`

The controller of one side of the duplex panel: the entry vista it holds and the reading view under it.
It finds the rows, takes the query, order and language sieve, and loads the chosen entry.
The reading view's deportment is built here, since a wing has no editor to own it.
The window view builds a wing over the atelier, whose ports it reads through.
The chosen entry is saved to the workspace state under the wing's side, so the next run reopens it.
It names no WPF type, so the match list, the keys and the focus live in `QWing`.

## `public event Action<string, Exception>? LWingFailed;`

Raised with the failure key when an entry load threw, so the veneer can report it.

## `public event Action? LWingLoaded;`

Raised once an entry is loaded, so the side re-lists its matches with the new mark.
A load that failed raises nothing, since the side stands where it stood.

## `public bool LWingFiltered`

Whether the vista hides any language, which the side shows as the mark on the sieve button.

## `public bool LWingQueried`

Whether the vista holds a query, so the side shows the list and its empty notice only then.

## `public void LWingOrderSet(CCatalogOrder? order)`

Hands a chosen ordering to the vista, which saves it under the side's tab and announces it.
A sender that is no order row hands null, which keeps the ordering it has.

## `public void LWingSieveSet(CCatalogFilter filter)`

Hands the ticked languages to the vista, which saves and announces them.

## `public void LWingSelect(long? id)`

Moves the vista's choice without loading, for the keys that walk the list.

## `public void LWingObserverAttach(Action<CBulletin> observer)`

Re-lists the matches on each vista, entry, reflex and settings announcement.
Order, filter or query moved, or a stored entry, a reflex fill or a flipped setting changed a listed row.
The side hands an observer that already runs on its thread, and each bulletin crosses as a shape.

## `public void LWingEntryShow(long? id)`

Restores one entry onto a freshly opened side, and a null id only clears the side.
The side is cleared first, so a failed load never leaves another tab's entry standing.

## `private void LWingEntryLoad(long id)`

Loads one entry onto this side.
An entry that is gone leaves the side empty rather than showing what it was.
A load that failed leaves the side where it stood and reports the failure.
The wing's display loads and chooses the entry, so the wing never holds the draft.
The wing then shows or clears its own lectern, so no veneer relays the draft.

## `public void LWingEntryOpen(long id)`

Loads the entry picked from the list and saves the side's standing.

## `public IReadOnlyList<CVistaRow> LWingRowsRead()`

The rows the engine returns for the vista, crossing as shapes so the side names no engine row.

## `private void LWingEntrySave()`

Writes the chosen entry to the left or right slot of the workspace state, whichever side this wing is.
The vista itself says which side it is, so nothing here copies that.
