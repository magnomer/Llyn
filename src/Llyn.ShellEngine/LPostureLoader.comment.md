# LPostureLoader.cs

## `public static class LPostureLoader`

Turns the posture text kept under the workspace into a state record and back.
It never touches the disk, since the keep port carries the text either way.
The keys are the ones the settings file carried before the posture moved out, so an old file reads as-is.
A window block holds the restored edges.
A layout block holds one object per tab under its lowercase name.

## `public static LPostureState LPostureLoaderRead(string? text)`

Nothing, a blank, or text that is not JSON reads as the default posture, so the window always opens.
Only an explicit `false` unlinks the panels, and only the editor word opens a split.
A volume outside the range is clamped, so a hand-edited number never reaches the player.
A missing number means full, so a workspace written before the setting existed plays as it did.

## `public static string LPostureLoaderFormat(LPostureState state)`

Persisted keys are a data contract, so they stay lowercase and independent of member names.
The split is written as one of two words rather than a boolean.
So a file read by hand says which side stood open.
A mode, a layout and a window are written only once each was set.
So the file says only what the user set.

## `private static LWindowState? LPostureWindowRead(JsonElement window)`

Any missing or unusable edge yields nothing at all.
Half a rectangle would place the window somewhere its owner never left it.

## `private static IReadOnlyList<LLayout> LPostureLayoutRead(JsonElement layout)`

A tab whose widths, ordering and filter are all missing or unusable is dropped rather than kept empty.
An unusable block as a whole yields an empty list, so every tab keeps its designed width.

## `private static Dictionary<string, object> LPostureLayoutCreate(IReadOnlyList<LLayout> layout)`

A width that was never dragged is left out.
So are an ordering never chosen and a filter never touched.
An ordering is written by its stored name, never by its position in the enumeration.

## `private static double? LPostureEdgeResolve(JsonElement window, string name)`

A window edge may be negative, since a screen left of the main one lies below zero.

## `private static double? LPostureWidthResolve(JsonElement tab, string name)`

A panel width below zero counts as missing, and so does an infinity.

## `private static double? LPostureNumberResolve(JsonElement block, string name)`

A hand-edited file may hold a number as text, so both forms are read.

## `private static LCatalogOrder? LPostureOrderResolve(JsonElement tab)`

An ordering is kept only when its stored name is one this build offers.
A name this build no longer knows counts as missing, and the panel opens on its designed ordering.

## `private static LCatalogFilter? LPostureFilterResolve(JsonElement tab)`

A filter is read as the joined text the catalog writes it in.
An empty text reads as the empty filter, which hides nothing.
