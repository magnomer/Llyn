# TPortraitClerk.cs
Hash: `c69cf83f9713e596`

## `public sealed class TPortraitClerk`

The portrait clerk composes a page over a real rig.
The press clerk then opens that page on the fake press.

## `public async Task PortraitClerkPrint_EntryPage_HandsTheSheetToThePressToPrint()`

The page of a stored entry is rendered as sheet HTML and opened on the press.
Nothing is printed, since only the print window asks the board to print.
The fact disposes the opened session clerk and its board, so no press session outlives it.
