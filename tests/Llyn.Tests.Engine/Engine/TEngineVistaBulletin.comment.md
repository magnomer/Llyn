# TEngineVistaBulletin.cs
Hash: `0a1ba9717c4eacd6`

## `public sealed class TEngineVistaBulletin`

Covers which vista changes raise the vista bulletin, on a real workspace.
It builds through `TWorkspace.TWorkspacePrepare`, attaches an observer, and starts its vista through the engine.

## `public void VistaOrderSet_ChangedOrder_RaisesVistaBulletin()`

A changed ordering raises one vista bulletin carrying the vista's own id.

## `public void VistaOrderSet_SameOrder_RaisesNothing()`

Setting the order, the filter, the query or the chosen row to what it already is raises no bulletin.
A repeated click therefore re-lists nothing.
