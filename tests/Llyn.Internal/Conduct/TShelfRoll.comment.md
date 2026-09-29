# TShelfRoll.cs

## `public sealed class TShelfRoll`

Covers the shelf's one rows answer on a real workspace.
The rows, the empty verdict and the chosen Source's worded tally arrive together, so the view reads once.
A chosen Source the query filters out closes before the tally is read, so the tally counts nothing.

## `private static LReference TShelfRollPrepare(LEngine engine)`

Stores a Source named Book and one entry whose sentence cites it once.
