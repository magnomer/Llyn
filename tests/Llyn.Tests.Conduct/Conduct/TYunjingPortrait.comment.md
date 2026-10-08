# TYunjingPortrait.cs
Hash: `1a706ef195a9944b`

## `public sealed class TYunjingPortrait`

Covers the yunjing panel's export and print of the chosen entry, on a real workspace.
It builds its panel through `TYunjing.TYunjingPrepare`.
Export writes nothing until an entry is chosen, and the chosen entry leaves the page for the reader.
Print does nothing while no entry is chosen.
