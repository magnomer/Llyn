# TPressWindowLens.cs
Hash: `6c39b5bb1931430c`

## `public sealed class TPressWindowLens`

Covers the print preview's zoom, owned by the galley's `CSLens`.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen` and drives the lens through its relays.
A fact needing the paper size draws a proof first and reads the size from the answer.

## `public async Task LensTextSet_CulturePercentSign_SetsFreeFactor()`

Typed 150 with the culture's percent sign gives a free factor of 1.5.
The answered text is that factor as the culture writes a percent.

## `public async Task LensTextSet_BareNumber_SetsFreeFactor()`

Typed 75 with no sign gives a free factor of 0.75.

## `public async Task LensTextSet_Word_RefitsCurrentMode()`

Typed text that is no number refits in the current mode, here a width fit.

## `public async Task LensLadderMove_Up_StepsToNextRung()`

From 100 percent, a raw wheel delta up steps to 110 and a step up then to 125.

## `public async Task LensLadderMove_Down_StepsToEarlierRung()`

From 80 percent, which lies between rungs, a step down lands on 75 and keeps the free mode.

## `public async Task LensLadderMove_TopRung_ClampsAtFourHundred()`

A typed 1000 clamps to 400 percent, and a step up stays there.

## `public async Task LensLadderMove_BottomRung_ClampsAtTen()`

A typed 1 clamps to 10 percent, and a step down stays there.

## `public async Task LensModeSet_FitWidth_ReturnsAreaAcrossPaper()`

A width fit answers the view width over the paper width.

## `public async Task LensModeSet_FitPage_ReturnsSmallerRatio()`

A page fit answers the smaller of the width and height ratios.

## `public async Task SightSet_ZoomPastDefault_RequestsHigherResolution()`

At 400 percent in a wide view, the sight asks for more than the default 300 dots per inch.
The delivered PNG is wider than the paper at 300 dots per inch, read from its header.
