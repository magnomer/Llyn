# TPressClerk.cs
Hash: `bacfa606e4abd25e`

## `public sealed class TPressClerk`

Covers the restore and save policy of the print setting clerk over an in-memory store.
An empty store restores nothing, so the session keeps its own setting.
A kept printer that is no longer listed falls back to the default printer.
A kept Save as PDF choice restores with no printer.
The page range comes back empty, and a save keeps the range empty.
The Conduct memory facts check the empty store and the dropped range through the window's gates.

## `public void PressClerkRestore_NothingStored_ReturnsNull()`

An empty store restores nothing.

## `public void PressClerkRestore_UnlistedPrinter_TakesDefaultPrinter()`

A stored printer missing from the list restores as the default printer.
A stored page range restores empty.

## `public void PressClerkRestore_SaveAsPdf_KeepsNoPrinter()`

A stored setting with no printer stays on Save as PDF.

## `public void PressClerkSave_TypedRange_StoresEmptyRange()`

A save stores the setting with its page range emptied and its scale kept.
