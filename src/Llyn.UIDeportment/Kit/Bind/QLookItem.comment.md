# QLookItem.cs
Hash: `f8c9e109a8bdd2c5`

## `internal static class QLookItem`

Fills item templates by code, now and after each regeneration.
This is the takeover of an item template's bindings, kept apart from the state switching in [QLook](QLook.comment.md).
It also holds the diff that makes a list's items match the engine's rows.

## `private static readonly ConditionalWeakTable<ItemsControl, Action<FrameworkElement, object, string?>> QLookList = [];`

Holds each attached list with its fill, so a second attach from a reloaded view is ignored.

## `private static readonly ConditionalWeakTable<FrameworkElement, Tuple<object, PropertyChangedEventHandler>> QLookWatch = [];`

Holds each container's item and the change handler hooked on it.

## `private static readonly ConditionalWeakTable<ItemsControl, HashSet<FrameworkElement>> QLookRoster = [];`

The containers each list's last scan found live, so a container gone from the list can be unhooked.

## `static QLookItem()`

Refills every attached list in full when the localization catalog changes.
A fill reads localized text once, so a language switch would otherwise leave rows in the old language.

## `internal static void QLookItemAttach(ItemsControl list, Action<FrameworkElement, object, string?> fill)`

Calls `fill` with every new container and its item, now and after each generation pass.
An item that raises a property change is filled again with that property's name.
The list is scanned again on each load, and its unload unhooks every container's item.

## `internal static void QLookItemApply(ItemsControl list)`

Fills every container of an attached list again, for a change the items do not raise themselves.

## `internal static void QLookItemShow<QLookItemRow, QLookItemDraft>(ObservableCollection<QLookItemRow> rows, IReadOnlyList<QLookItemDraft> drafts, Func<QLookItemRow, long?> key, Func<QLookItemDraft, long> id, Func<QLookItemDraft, QLookItemRow> create, Func<QLookItemRow, QLookItemDraft, QLookItemRow> update)`

The one diff every engine list a driver shows renders through.
The engine holds the list and owns its order, and the driver only copies it.
So a bulletin brings the whole list back, and only what differs may change.
Makes `rows` show `drafts` in their order, duplicates included, around any row it does not own.
Each draft claims the first unclaimed row carrying its id, so pairing is one to one.
A draft with no such row is built with `create`.
`update` may answer a fresh object, which then takes the paired row's place.
Only removal by identity and appending are used, because the engine alone owns the order.
The rows kept untouched are the longest head of the new order found, in order, among the standing rows.
They are compared by object identity, and each keeps its object and its control.
Every other engine row is removed and re-added.
Its object survives, but its control is rebuilt and loses focus and caret.
This includes rows the change never passed.
Moving B to the front of A, B, C, D rebuilds A, C and D.
An insert in the middle rebuilds every later row.
Moving a row to the end rebuilds only that row.
That cost is the price of never inserting or moving.
A fresh object from `update` counts as a mismatch, so it and every later row are re-added.
Conduct does not promise unique ids, so repeated ids show faithfully and never throw.
Removing repeats is behaviour, owned by Conduct or a layer below it, never by the driver.
A row `key` answers null for is not the engine's, so it is never removed or re-added.
It keeps its place among the rows, and the drafts show in order around it.
No caller holds such a row today, because every key answers an id.

## `private static void QLookItemScan(ItemsControl list, Action<FrameworkElement, object, string?> fill, bool full)`

Walks the containers by index, so equal items each get their own container filled.
Fills each container whose item is new to it and hooks the item's changes.
A full pass fills the known containers too.
A container that took another item drops the old item's handler.
Containers no longer live are unhooked after the walk.

## `private static void QLookItemTeardown(ItemsControl list, HashSet<FrameworkElement> live)`

Unhooks the item handler of every rostered container missing from `live`, then records `live` as the roster.
An empty `live` unhooks the whole list, which its unload uses.
