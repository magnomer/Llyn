# LPressClerk.cs
Hash: `2bfee7300a83c3a3`

## `public sealed class LPressClerk`

The print setting rules a print session runs, so ShellEngine names no Core rule.
It holds no session state, only the store port taken from the rig.
Apart from restore and save, every member takes its inputs as data and answers a new value.
The Core records keep the facts intrinsic to them, and this clerk is their only caller in the engine.
`LWorkspaceStaff` builds one per rig, so a workspace change swaps the store with the rest.

## `public LPressClerk(LRig rig)`

Binds the print setting store of the rig.

## `public LPressSetting LPressClerkCreate(IReadOnlyList<LPressPrinter> printers)`

The opening setting for the default printer, else the first listed, else Save as PDF.
An empty list therefore answers the Save as PDF setting a session starts with.

## `public LPressSetting LPressClerkSelect(LPressSetting setting, IReadOnlyList<LPressPrinter> printers, string? name)`

The setting moved to the named printer and clamped against it.
A null name means Save as PDF.
A name the list lacks throws, since the window offers only listed printers.

## `public LPressSetting LPressClerkClamp(LPressSetting setting, IReadOnlyList<LPressPrinter> printers)`

The setting made valid for the printer it names, found in the list.
A name the list lacks clamps as Save as PDF would.

## `public LPressSetting? LPressClerkRestore(IReadOnlyList<LPressPrinter> printers)`

The kept setting fitted to the listed printers, or null when the store holds none.
A kept printer stays only while listed, else the default or first listed printer stands in.
A kept null printer stays Save as PDF.
The page range comes back empty, since pages belong to one document.
A store that cannot be read raises `LVaultFault`, which the caller shows.

## `public void LPressClerkSave(LPressSetting setting)`

Keeps the setting for the next print window, with its page range emptied.
A store that cannot be written raises `LVaultFault`.

## `public LPressRange? LPressClerkParse(string text, int count)`

The range the typed text names against `count` pages, or null when the text is invalid.

## `public LPressMargin LPressClerkResolve(double top, double bottom, double left, double right, bool metric)`

Typed margin values in the reader's unit, as a preset when they equal one, else custom.

## `public LPressMargin LPressClerkResolve(LPressMargin margin, string? name)`

The named preset, or the current values renamed custom when the name is null.
A name no preset carries throws, since the window offers only presets.

## `public LPressDigest LPressClerkRead(LPressSetting setting, IReadOnlyList<LPressPrinter> printers, int? count)`

The ready facts for the window, given the drawn page count or null before a draw.
Two-sided printing halves the sheet count, rounding up, before copies multiply it.

## `private static LPressPrinter? LPressClerkFind(IReadOnlyList<LPressPrinter> printers, string? name)`

The listed printer of that name, or null for Save as PDF or an unlisted name.

## `private static LPressPrinter? LPressClerkFind(IReadOnlyList<LPressPrinter> printers)`

The default printer, else the first listed, else null when none is installed.
