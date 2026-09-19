# TPanelMode.cs

## `public sealed class TPanelMode`

Covers the mode the shared panel state switches between, the viewer and the scribe.
Leaving the scribe with unstored changes asks the leave seam once, and a refusal keeps the scribe.
A clean editor never asks.
The fresh request drops the chosen row and opens the scribe on nothing.
