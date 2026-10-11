# TPressBoard.cs
Hash: `e9775568b2cd1543`

## `internal sealed class TPressBoard : LPressBoard`

A board that holds no browser, answered by `TPress` when a print session opens.
Tests reach the session route without a browser engine or a printer.
Every task completes before the call returns unless a test sets a hold.
The board records what it was handed.
Draws and prints run one at a time, as the Windows twin runs them.
The session clerk relies on that order, so the fake keeps it.

## `private readonly SemaphoreSlim _tPressBoardTurn`

The turn a draw or a print takes for its whole run, held across any hold.
A second call therefore waits, and records its setting only once its turn starts.

## `internal LPressSetting? TPressBoardSetting`

The setting of the last draw, print or export, or null before any.

## `internal string? TPressBoardPath`

The path of the last export, or null when nothing was exported.

## `internal bool TPressBoardShown`

Whether the system print dialog was asked for at least once.

## `internal int TPressBoardCount`

How many proofs were drawn, so a test sees whether a draw was skipped.

## `internal bool TPressBoardDisposed`

Whether the board was disposed, so a test sees the session release it.

## `internal LPressStatus TPressBoardStatus`

The status a print answers, done until a test sets it.
So a test can make the printer offline or failing.

## `internal TaskCompletionSource? TPressBoardHold`

A gate that draws and prints wait on until the test releases it.
Null by default, so every task completes at once.
So a test can hold a draw open while a newer setting arrives.

## `internal List<TPressProof> TPressBoardProof`

Every proof the board drew, in order, so a test sees which ones were disposed.

## `internal Func<LPressProof, LPressProof>? TPressBoardCover`

Wraps each proof the board hands out, when a test sets it.
The fault sweep wraps the proof in a fault double, so a page draw can fail on demand.
The list above still keeps the bare proof.

## `public async Task<LPressProof> LPressBoardDraw(LPressSetting setting)`

Waits its turn, records the setting and counts the draw, then waits for any hold.
Answers three blank pages on the chosen paper, turned when the orientation is landscape.

## `public async Task<LPressStatus> LPressBoardPrint(LPressSetting setting)`

Waits its turn, records the setting, waits for any hold and answers `TPressBoardStatus`.

## `public Task LPressBoardExport(LPressSetting setting, string path)`

Records the setting and the path, and writes no file.

## `public Task LPressBoardShow()`

Marks the dialog as shown and opens none.

## `public ValueTask DisposeAsync()`

Marks the board as disposed, since nothing is held open.
