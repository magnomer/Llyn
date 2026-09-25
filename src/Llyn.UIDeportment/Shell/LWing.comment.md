# LWing.cs

## `public sealed class LWing`

The deportment of one side of the duplex panel: the entry vista it holds and the reading view under it.
It finds the rows, takes the query, order and language sieve, and loads the chosen entry.
It owns the side's match list and steers it from the field's keys, focus and clicks.
The reading view's deportment is built here, since a wing has no editor to own it.
The chosen entry is saved to the workspace state under the wing's side, so the next run reopens it.

## `private LIndex? _lWingIndex;`

The side's match list, absent until the veneer hands over its controls.
Every use tolerates its absence, so an announcement before the attach does nothing.

## `private TextBox? _lWingQuery;`

The side's search field, read on each keystroke and kept to tell a leave from a move into the list.

## `public event Action<string, Exception>? LWingFailed;`

Raised with the failure key when an entry load threw, so the veneer can report it.

## `public event Action<LEntryDraft>? LWingDraftChanged;`

Raised with the loaded entry, so the reading view shows it.

## `public event Action? LWingCleared;`

Raised when the side stands on nothing, so the reading view empties.

## `private void LWingOrderSet(LCatalogOrder? order)`

Hands a chosen ordering to the vista, which saves it under the side's tab and announces it.
A sender that is no order row hands null, which keeps the ordering it has.

## `private void LWingSieveSet(LCatalogFilter filter)`

Hands the ticked languages to the vista, which saves and announces them.

## `public void LWingOrderHandle(object sender, ToggleButton dropper)`

A clicked order row closes the dropdown and sets the ordering its tag carries.
The announcement then re-lists the matches.

## `public void LWingSieveHandle(Panel list, UIElement mark)`

A clicked language row sets the filter the list now stands for, then redraws the mark at once.

## `public void LWingSieveShow(UIElement mark)`

Shows the mark on the sieve button while the vista hides any language.

## `public void LWingIndexAttach(ItemsControl view, FrameworkElement empty, TextBox query, Func<string, ImageSource?> flagSeam)`

Takes the side's list, empty notice and field once, and builds the index over them.

## `public void LWingObserverAttach(DispatcherObject surface, Func<DispatcherObject, Action, Action<LBulletin>> observerSeam)`

Re-lists the matches on each vista, entry, reflex and settings announcement.
Order, filter or query moved, or a stored entry, a reflex fill or a flipped setting changed a listed row.
The seam puts each response on the surface's thread.

## `private void LWingIndexShow()`

Lists the matches from the vista, already filtered, sorted, numbered and marked by the engine.
The empty notice shows only while a typed query matched nothing.

## `public void LWingIndexClear()`

Empties the match list.

## `private void LWingIndexHide()`

Collapses the match list and leaves the query standing.

## `public void LWingEntryShow(long? id)`

Restores one entry onto a freshly opened side, and a null id only clears the side.
The side is cleared first, so a failed load never leaves another tab's entry standing.

## `private void LWingEntryLoad(long id)`

Loads one entry onto this side for a pick from the list.
An entry that is gone leaves the side empty rather than showing what it was.
A load that failed leaves the side where it stood and reports the failure.
The vista's choice follows what the side now stands on.

## `public void LWingQueryHandle()`

Hands the text to the vista, then shows the list while the vista holds a query and hides it otherwise.

## `public void LWingKeyHandle(KeyEventArgs e)`

The keyboard path from the field into the open list.
A closed list lets every key through to the field, so a hidden list is never picked from.
The index remembers whether it is shown, since a control's state never gates a request.
Escape hides the list and takes the key.
Enter picks the chosen row while the rows still mark it.
Down and Up move the choice one row, stopping at either end.
The rows are re-marked in place and the chosen one scrolled into view.

## `public void LWingLeaveHandle(KeyboardFocusChangedEventArgs e)`

Focus leaving the field and the list hides the list.
Focus moving between the two is not a leave, since a clicked row takes focus before its click lands.

## `public void LWingIndexHandle(object sender)`

A clicked row hides the list, shows its entry and saves the side's standing.

## `private void LWingEntrySave()`

Writes the chosen entry to the left or right slot of the workspace state, whichever side this wing is.
The vista itself says which side it is, so nothing here copies that.
