# TGalleyClerk.cs
Hash: `4c8f6ac4cbc96a5a`

## `public sealed class TGalleyClerk`

The print session clerk over the fake press board, opened on a stored entry's page.
The board serialises its draws as the Windows twin does, so the clerk's latest-wins rule is tested alone.

## `public async Task GalleyClerkDraw_TwoDrawsBackToBack_KeepsNewerProof()`

Two draws start back to back while the board holds the first.
The older draw answers false and its proof is disposed.
The newer landscape proof is the one held, and it stays open.
This is the proof that latest-wins needs no turn chain above the board.

## `public async Task GalleyClerkDraw_NoResolution_DrawsDefaultResolution()`

A page asked with no resolution comes back at 300 dots per inch of its paper.
The width is read from the PNG header, since the image carries no size of its own.

## `public async Task GalleyClerkDraw_GivenResolution_DrawsAtResolution()`

A page asked at 50 dots per inch comes back that wide per inch of paper.
It is taller than wide because the fake proof keeps the portrait paper's ratio.

## `public async Task GalleyClerkDraw_BeforeProof_ReturnsNull()`

A page asked before any proof is drawn answers null, since nothing is held to read.

## `public async Task GalleyClerkDraw_PageBeyondCount_Throws()`

A page past the proof's count throws, which the gate above shows as `Print.RenderFailed`.

## `public async Task GalleyClerkDraw_ProofReplacedMidPage_ReturnsNull()`

A newer draw replaces the proof while one of its pages is still drawing.
The replaced proof stays open until that page draw ends, then it is disposed.
The page draw answers null, since a replaced proof is latest-wins and no failure.

## `private static async Task<LGalleyClerk> TGalleyClerkOpen(TWorkspace workspace, TPressBoard board)`

Opens a session clerk on `board` for the page of a freshly stored entry.
