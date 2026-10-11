# CSPressWording.cs
Hash: `f68774bb39cc60f9`

## `internal static class CSPressWording`

The read-only mapping from the galley's view to the print window's Conduct records.
It holds no state, so `CSPress` keeps every field its gates change.
It chooses each wording key from a view fact, since wording is the window's concern.
Every Core enum maps through an exhaustive switch, so a new member throws instead of slipping through.

## `internal static CSPressSetting LSPressWordingRead(LGalleyView view, int page, bool running)`

Maps one galley view and the window's own state, shared by the read and restore gates.
An invalid typed range shows `Print.PagesInvalid`, and the margin unit follows the metric flag.
The button reads `Print.Save` for Save as PDF, else `Print.Print`.
A printer that did not report its options shows `Print.CapabilityUnknown`.
The sheet count wording is singular for exactly one sheet and plural otherwise.
The footer reads `Print.Printing` while a print runs, else `Print.Idle`.
The page and the running flag come from the caller, which owns them.

## `internal static string? LSPressWordingRead(LPressStatus status)`

The wording key of a print status, or null when the print or save finished.

## `private static CSPressPrinter LSPressWordingRead(LGalleyDestination printer)`

One destination of the galley view as a ready row.
Save as PDF reads `Print.SaveAsPdf` and the system default printer reads `Print.PrinterDefault`.
Any other printer shows its queue name with braces doubled.
The window formats either with the queue name, so it shows one value without choosing.

## `private static CSPressPaper LSPressWordingRead(LGalleyStock paper)`

One sheet of the galley view as a ready row with its wording key.
A null name is a custom sheet.
Every sheet name comes from the Core standard list.
So a name outside the mapped set means Core grew a sheet this map lacks, and it throws.

## `private static List<CSPressChoice<CSPressRange>> LSPressWordingRead(LPressScope scope)`

The pages modes as ready rows, with the mode the scope stands for chosen.
The rows follow the enum's declared order, which is the only order the picker and its gate share.

## `private static List<CSPressChoice<CSPressMargin>> LSPressWordingRead(string? name)`

The margin presets and Custom as ready rows, with the one a Core margin name stands for chosen.
A null name means Custom.
An unknown name throws, so a new Core preset cannot slip through unmapped.

## `private static List<CSPressChoice<CSPressSide>>? LSPressWordingRead(LPressSide? side)`

The two-sided modes as ready rows, with the mode the setting uses chosen.
A null side means Save as PDF, which offers no two-sided choice, so the list is null too.
