# LWindowLoader.cs

## `public static class LWindowLoader`

Reads and writes the window block of the settings file.
It owns only that block, so `LSettingsLoader` stays the reader of the file as a whole.

## `public static LWindowState? LWindowLoaderRead(JsonElement window)`

Any missing or unusable edge yields nothing at all.
Half a rectangle would place the window somewhere its owner never left it.

## `public static Dictionary<string, object> LWindowLoaderCreate(LWindowState window)`

Builds the block as plain keys, which are a data contract and stay lowercase.

## `private static double? LWindowLoaderResolve(JsonElement window, string name)`

A hand-edited file may hold a number as text, so both forms are read.
Anything else, including an infinity, counts as missing.
