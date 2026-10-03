# TYunjingPortrait.cs
Hash: `410e0b708aa26692`

## `public sealed class TYunjingPortrait`

Covers the yunjing panel's export and print of the chosen entry, on a real workspace.
It builds its panel through `TYunjing.TYunjingPrepare`.
Export writes nothing until an entry is chosen, and the chosen entry leaves the page for the reader.
Print does nothing while no entry is chosen.
