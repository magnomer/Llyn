# TShelfRoll.cs
Hash: `807cccc93f5c7ea3`

## `public sealed class TShelfRoll`

Covers the shelf's one rows answer on a real workspace.
The rows, the empty verdict and the chosen Source's worded tally arrive together, so the view reads once.
A chosen Source the query filters out closes before the tally is read, so the tally counts nothing.
Each row words its authors and year ready: the text, or the unknown or unset key.
A credited author shows even while the authorship is marked unknown.

## `private static LReference TShelfRollPrepare(LEngine engine)`

Stores a Source named Book and one entry whose sentence cites it once.

## `private static LReference TShelfWordingPrepare(LEngine engine, string title, LStateValue year, LStateMark authors)`

Stores a Source under the title with the year and the authorship state given, crediting nobody.
