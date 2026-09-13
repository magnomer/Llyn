# LLayoutLoader.cs

## `public static class LLayoutLoader`

Reads and writes the layout block of the settings file.
It owns only that block, so `LSettingsLoader` stays the reader of the file as a whole.
The block is an object keyed by tab name, each holding the widths of that tab's fixed panels.
Beside the widths sit the ordering the tab lists by and the languages it hides, under `order` and `filter`.

## `public static IReadOnlyList<LLayout> LLayoutLoaderRead(JsonElement layout)`

A tab whose widths, ordering and filter are all missing or unusable is dropped rather than kept empty.
An unusable block as a whole yields an empty list, so every tab keeps its designed width.

## `public static Dictionary<string, object> LLayoutLoaderCreate(IReadOnlyList<LLayout> layout)`

Builds the block as plain keys, which are a data contract and stay lowercase.
A width that was never dragged is left out, so the file says only what the user set.
So are an ordering never chosen and a filter never touched.
An ordering is written by its stored name, never by its position in the enumeration.

## `private static double? LLayoutLoaderResolve(JsonElement tab, string name)`

A hand-edited file may hold a number as text, so both forms are read.
Anything else, including an infinity or a negative width, counts as missing.

## `private static LCatalogOrder? LLayoutOrderResolve(JsonElement tab)`

An ordering is read by its stored name and kept only when that name is one this build offers.
A name this build no longer knows counts as missing.
The panel then opens on the ordering it was designed with.

## `private static LCatalogFilter? LLayoutFilterResolve(JsonElement tab)`

A filter is read as the joined text the catalog writes it in.
An empty text reads as the empty filter, which hides nothing.
