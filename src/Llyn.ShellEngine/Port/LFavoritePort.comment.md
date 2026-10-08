# LFavoritePort.cs
Hash: `3c243051779a80d9`

## `public interface LFavoritePort`

The slice of the engine a deportment sees when it lists or marks favourite entries.
`LVistaFacade` implements it, since favourites are listed through a vista.

## `IReadOnlyList<LVistaRow> LEngineFavoriteFind(LVista vista);`

The favourite rows the vista lists, with its query, order and filter applied.

## `bool LEngineFavoriteCheck(long entryId);`

Whether the entry is marked as a favourite.

## `void LEngineFavoriteSave(long entryId);`

Marks the entry as a favourite.

## `void LEngineFavoriteDelete(long entryId);`

Removes the favourite mark from the entry.
