# CSGalleyWindow.cs
Hash: `b2e9a298de0b6e59`

## `internal sealed record CSGalleyWindow(int CSGalleyWindowTurn, int CSGalleyWindowCount, int CSGalleyWindowLow, int CSGalleyWindowHigh, int? CSGalleyWindowResolution, IReadOnlySet<CSGalleyLeaf> CSGalleyWindowHeld)`

The galley's sight window over one proof and the pictures delivered inside it.
It is immutable, and the galley replaces it whole, so its state has one writer.
Each change answers a new window, and the galley compares old and new to announce dropped pages.

**Parameters**

- `CSGalleyWindowTurn`: counts finished draws.
  A page answer whose request began under an older turn is dropped, so no old page reaches a new proof.
- `CSGalleyWindowCount`: the page count of the proof, which bounds the window.
  It is zero before any draw and after a failed one, so a sight then requests nothing.
- `CSGalleyWindowLow`: the first page of the window the latest valid sight asked for.
- `CSGalleyWindowHigh`: the last page of that window, or -1 before any valid sight on the current proof.
- `CSGalleyWindowResolution`: the dots per inch the latest sight asked for, or null for the default.
- `CSGalleyWindowHeld`: the delivered pictures, at most one per page and never outside the window.

## `internal CSGalleyWindow LSGalleyWindowMove(int first, int last, int? resolution)`

Answers the window over the visible range `first` to `last`, one page either side, clipped to the proof.
It keeps only the held pictures inside the new bounds.
A held picture at another resolution stays until its replacement arrives, so the sheet never blanks.

## `internal bool LSGalleyWindowCheck(CSGalleyLeaf leaf)`

Answers whether the window still wants `leaf`.
That is a page inside the bounds, at the window's resolution, and not yet held.
The galley asks it before a request and again before it delivers the answer.

## `internal CSGalleyWindow LSGalleyWindowAdd(CSGalleyLeaf leaf)`

Answers the window holding `leaf` in place of any other picture of the same page.
So the held set mirrors exactly one picture per page, as the driver shows it.
