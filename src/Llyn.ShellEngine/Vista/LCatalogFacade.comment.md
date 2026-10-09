# LCatalogFacade.cs
Hash: `8545ab05f0c69039`

## `public sealed class LCatalogFacade : LTagPort, LRegisterPort, LFavoritePort`

The engine's facade for the catalog shelves a card or an entry is filed under: Tags, Registers and favorites.
A call that reaches a clerk takes the gate and hands the work to the catalog clerk owning the rows.
It implements the tag, register and favourite ports itself, so Host hands it to Conduct with no outlet between.

## `internal LCatalogFacade(LEngineHearth hearth, LVistaRowFacade row)`

Stores the hearth, its gate and the row builder, all built by `LEngine` before this one.
The gate, the staff and the bulletins are read through the hearth.
It names no sibling facade, since every shelf it serves sits in the catalog staff.

## `internal IReadOnlyList<LTag> LEngineTagRead()`

Reads every Tag the workspace holds, once each, in alphabetical order.

## `public IReadOnlyList<LTag> LEngineTagFind(string query, LCatalogOrder order)`

The stored Tags answering `query`, in `order`, as the taxonomy vista lists them.

## `public LTagOffer LEngineTagFind(LTenure held, long card, string text)`

The stored Tags a card's tag field offers for the text it keeps.
Those the held draft's card already shows are left out.
The draft is read before the gate is taken, as the tenure guards itself.

## `public IReadOnlyList<LCatalogTag> LEngineTagFind(LVista vista)`

The tags the taxonomy panel's vista lists, with the query and order read off the vista.
The vista's filter hides languages from the entries of the chosen tag, not tags, so it is not applied here.
A vista whose chosen tag no longer answers is deselected.

## `public LTag LEngineTagCreate(string text)`

The tag clerk's create, then the tag bulletin raised outside the gate.
The catalog is announced so every panel listing Tags shows the new row.

## `public LRegisterOffer LEngineRegisterFind(LTenure held, long card, string text)`

The stored Registers a card's register field offers for the text it keeps.
Those the held draft's card already links are left out.
The language whose pack seeds the shelf is the held draft's own, so no caller passes one.
The draft is read before the gate is taken, as the tenure guards itself.

## `public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(string query, LCatalogOrder order)`

The stored Registers answering `query`, in `order`, as the tenor vista lists them.

## `public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista)`

The shelf the tenor panel's vista lists, with the query and order read off the vista.
The vista's filter hides languages from the entries of the chosen Register, not Registers, so it is not applied here.
A vista whose chosen Register no longer answers is deselected.

## `public LRegister LEngineRegisterCreate(string name)`

The register clerk's create, then the register bulletin raised outside the gate.
The announcement is raised after the lock is released, so the panel lists and selects the row.

## `public IReadOnlyList<LCatalogFavorite> LEngineFavoriteFind(string query, LCatalogOrder order, LCatalogFilter filter)`

The marked entries matching the query in the given order, with those in a hidden language left out.

## `public IReadOnlyList<LVistaRow> LEngineFavoriteFind(LVista vista)`

The rows the favorites panel's vista lists, with the query, order and filter read off the vista.
They come back as vista rows, twins numbered and epithets read in one scan, so the panel only copies them.
The row of the entry the vista stands on comes back marked chosen, so the panel keeps no choice.

## `public bool LEngineFavoriteCheck(long entryId)`

Whether the entry is marked.

## `public void LEngineFavoriteSave(long entryId)`

Marks the entry a favorite and announces the mark.
Marking changes no lexical data and keeps the entry's identity.

## `public void LEngineFavoriteDelete(long entryId)`

Unmarks the entry and announces it.
The entry stands, still reachable through the entry catalog.
