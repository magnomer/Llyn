# LLayoutLoader.cs

## `public static class LLayoutLoader`

Reads and writes the layout block of the settings file.
It owns only that block, so `LSettingsLoader` stays the reader of the file as a whole.
The block is an object keyed by tab name, each holding the widths of that tab's fixed panels.

## `public static IReadOnlyList<LLayout> LLayoutLoaderRead(JsonElement layout)`

A tab whose widths are all missing or unusable is dropped rather than kept empty.
An unusable block as a whole yields an empty list, so every tab keeps its designed width.

## `public static Dictionary<string, object> LLayoutLoaderCreate(IReadOnlyList<LLayout> layout)`

Builds the block as plain keys, which are a data contract and stay lowercase.
A width that was never dragged is left out, so the file says only what the user set.

## `private static double? LLayoutLoaderResolve(JsonElement tab, string name)`

A hand-edited file may hold a number as text, so both forms are read.
Anything else, including an infinity or a negative width, counts as missing.
