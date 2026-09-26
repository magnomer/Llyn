# PLookItem.cs

## `internal static class PLookItem`

Fills item templates by code, now and after each regeneration.
This is the takeover of an item template's bindings, kept apart from the state switching in [PLook](PLook.comment.md).

## `private static readonly ConditionalWeakTable<`

`PLookList` holds each attached list with its fill, so a second attach from a reloaded view is ignored.
`PLookWatch` holds each container's item and the change handler hooked on it.

## `private static readonly ConditionalWeakTable<ItemsControl, HashSet<FrameworkElement>> PLookRoster = [];`

The containers each list's last scan found live, so a container gone from the list can be unhooked.

## `static PLookItem()`

Refills every attached list in full when the localization catalog changes.
A fill reads localized text once, so a language switch would otherwise leave rows in the old language.

## `internal static void PLookItemAttach(ItemsControl list, Action<FrameworkElement, object, string?> fill)`

Calls `fill` with every new container and its item, now and after each generation pass.
An item that raises a property change is filled again with that property's name.
The list is scanned again on each load, and its unload unhooks every container's item.

## `internal static void PLookItemApply(ItemsControl list)`

Fills every container of an attached list again, for a change the items do not raise themselves.

## `private static void PLookItemScan(ItemsControl list, Action<FrameworkElement, object, string?> fill, bool full)`

Walks the containers by index, so equal items each get their own container filled.
Fills each container whose item is new to it and hooks the item's changes.
A full pass fills the known containers too.
A container that took another item drops the old item's handler.
Containers no longer live are unhooked after the walk.

## `private static void PLookItemDetach(ItemsControl list, HashSet<FrameworkElement> live)`

Unhooks the item handler of every rostered container missing from `live`, then records `live` as the roster.
An empty `live` unhooks the whole list, which its unload uses.
