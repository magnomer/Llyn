# TEngineVistaRelay.cs
Hash: `0b2dfd37ce1ff770`

## `public sealed class TEngineVistaRelay`

Covers the relays a panel reads its vista through, first with no vista and then with one.
It builds through `TWorkspace.TWorkspacePrepare` and starts any vista through the engine.

## `public void VistaOrderRead_NoVista_ReadsHeadwordOrder()`

A panel with no vista yet reads the headword order.

## `public void VistaStoredCheck_OrphanRowOrNone_IsNotStored()`

No row and the zero row the engine words itself name no stored record, and a positive id does.

## `public void VistaFilterRead_NoVista_ReadsEmptyFilter()`

A panel with no vista yet reads the shared empty filter.

## `public void VistaFilterSet_NothingHidden_KeepsSharedEmptyFilter()`

Hidden languages set on the vista are kept as given.
Hiding nothing sets the engine's shared empty filter.

## `public void VistaUsageRead_CreditedAuthor_CountsItsWorks()`

An Author reaches the Sources that credit it, and nothing while no row is chosen.

## `public void VistaUsageRead_ChosenEntry_CountsNothing()`

A chosen entry is referenced by no other record, so its count is zero.
