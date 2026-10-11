# TInterfaceSettings.cs
Hash: `53890c16052cbbfe`

## `internal static partial class TInterface`

The relays for the settings file and the posture the shell keeps beside it.
The settings record is built here with each switch named, custom analysis on unless a test turns it off.
The layout and catalog filter records are built here too.
The settings loader is relayed here, so the file contract can be checked without an engine.
The posture file adapter is relayed the same way, so the posture text contract can be checked without a keep.
The adapter itself is built here over a keep file, and its read and save are relayed.
The trail adapter is built and relayed here too, since it stands beside the posture in the rig.
The print setting file is built and relayed the same way, since it keeps its JSON beside the posture.
The print setting clerk is built over a fake rig with a given store.
So its restore policy is checked alone.
The opening setting, a typed page range and the fake queue's printers are relayed for those checks.
The posture itself is started over an engine and pushed a field at a time, as the atelier pushes it.
The engine's left and right entry saves are relayed here too.
Each relay is transparent and carries no test logic of its own.

## `internal static LSettings TSettingsCreate(string localization, bool respelled = false, bool frequency = true, bool morphology = true, bool epithet = true, bool tally = false, string gloss = "English", bool analysis = true)`

Builds the settings record with every switch named.
Custom analysis is on unless `analysis` turns it off.

## `internal static LLayout TLayoutCreate(string tab, LCatalogOrder? order = null, LCatalogFilter? filter = null)`

Builds one tab's layout, with no order and no filter unless given.

## `internal static LCatalogFilter TCatalogFilterCreate(params string[] hidden)`

Builds a catalog filter that hides the `hidden` names.

## `internal static bool TSettingsExist(string root)`

Relays the loader's check that a settings file stands under `root`.

## `internal static LSettings TSettingsLoad(string root)`

Relays the loader's read of the settings file under `root`.

## `internal static void TSettingsSave(string root, LSettings settings)`

Relays the loader's save of `settings` under `root`.

## `internal static LPostureState TPostureStateCreate(IReadOnlyList<LLayout>? layout = null, string? mode = null, bool split = false, double volume = 1)`

Builds a posture state, unsplit at full volume unless given.

## `internal static LPostureFile TPostureFileCreate(string root)`

Builds the posture file adapter over a keep file at `root`.

## `internal static LPostureState? TPostureFileRead(this LPostureFile file, string name)`

Relays the adapter's read of the posture kept as `name`.

## `internal static void TPostureFileSave(this LPostureFile file, string name, LPostureState state)`

Relays the adapter's save of `state` as `name`.

## `internal static LPressFile TPressFileCreate(string root)`

Builds the print setting file over a keep file at `root`.

## `internal static LPressSetting? TPressFileRead(this LPressFile file)`

Relays the print setting file's read.

## `internal static void TPressFileSave(this LPressFile file, LPressSetting setting)`

Relays the print setting file's save.

## `internal static LPressSetting? TPressFileParse(string? text)`

Relays the print setting parse, so the JSON contract is checked without a file.

## `internal static LPressSetting TPressSettingCreate(LPressPrinter? printer)`

Relays the opening setting for `printer`, or for Save as PDF when it is null.

## `internal static LPressRange TPressRangeCreate(string text)`

Builds a page range from its typed text.

## `internal static IReadOnlyList<LPressPrinter> TPressPrinterRead()`

Answers the fake queue's two printers, `Office Laser` as the default and `Plotter`.

## `internal static LPressClerk TPressClerkCreate(LPressVault vault)`

Builds the print setting clerk over a fake rig whose store is `vault`.

## `internal static LPressSetting? TPressClerkRestore(this LPressClerk clerk, IReadOnlyList<LPressPrinter> printers)`

Relays the clerk's restore against the listed `printers`.

## `internal static void TPressClerkSave(this LPressClerk clerk, LPressSetting setting)`

Relays the clerk's save.

## `internal static LTrail TTrailCreate()`

Builds the system trail adapter.

## `internal static string? TTrailResolve(this LTrail trail, string root, string path)`

Relays the trail's resolve of `path` under `root`.

## `internal static string? TTrailRelativeResolve(this LTrail trail, string root, string path)`

Relays the trail's relative resolve of `path` against `root`.

## `internal static string TTrailNameRead(this LTrail trail, string path)`

Relays the trail's name read.

## `internal static bool TTrailRootCheck(this LTrail trail, string path)`

Relays the trail's root check.

## `internal static string TTrailNameNormalize(this LTrail trail, string name)`

Relays the trail's name normalizing.

## `internal static LPostureState TPostureLoaderRead(string? text)`

Relays the posture text parse.

## `internal static string TPostureLoaderFormat(LPostureState state)`

Relays the posture text format.

## `internal static LPosture TPostureStart(this LEngine engine)`

Starts the posture over `engine`.

## `internal static LPostureState TPostureRead(this LPosture posture)`

Relays the posture read.

## `internal static void TPostureVolumeSet(this LPosture posture, double volume)`

Relays the volume change without a save.

## `internal static void TPostureVolumeSave(this LPosture posture, double volume)`

Sets the volume and then saves it, the two steps the atelier takes.

## `internal static void TPostureModeSave(this LPosture posture, string mode)`

Relays the mode save.

## `internal static void TPostureLayoutSave(this LPosture posture, params LLayout[] layout)`

Relays the layout save.

## `internal static void TEngineLeftSave(this LEngine engine, long? id)`

Relays the workspace's save of the left entry.

## `internal static void TEngineRightSave(this LEngine engine, long? id)`

Relays the workspace's save of the right entry.
