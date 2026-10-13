# Press

This folder tests the print session clerk over the fake press board.
It stands apart from the portrait tests because it pins session behaviour and not page content.

## `TGalleyClerk`

The clerk is opened on a stored entry's page.
The board serialises its draws as the Windows twin does, so the clerk's latest-wins rule is tested alone.

Two draws start back to back while the board holds the first.
The older draw answers false and its proof is disposed.
The newer landscape proof is the one held, and it stays open.
So latest-wins needs no turn chain above the board.

A page asked with no resolution comes back at 300 dots per inch of its paper.
The width is read from the PNG header, since the image carries no size of its own.
A page asked at 50 dots per inch comes back that wide per inch of paper.
It is taller than wide because the fake proof keeps the portrait paper's ratio.

A page asked before any proof is drawn answers null, since nothing is held to read.
A page past the proof's count throws, which the gate above shows as `Print.RenderFailed`.
A newer draw can replace the proof while one of its pages is still drawing.
The replaced proof stays open until that page draw ends, then it is disposed.
The page draw answers null, since a replaced proof is latest-wins and no failure.

### `TGalleyClerkOpen`

Opens a session clerk on the board for the page of a freshly stored entry.

## Stamp

Commit: `c2f335c331fe`
- `TGalleyClerk.cs` `4c8f6ac4cbc96a5a`
