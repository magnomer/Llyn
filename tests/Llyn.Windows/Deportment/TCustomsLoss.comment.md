# TCustomsLoss.cs

## `public sealed class TCustomsLoss`

Covers the card count the customs window shows for a Replace.
Every card is counted, nested children included.
It also covers the first choice a row takes and when a row is ready.
A New row's intake drops the pick it still holds, so the engine sees no target.
It covers the loss text, which only a Replace with a stored target and a window produces.
