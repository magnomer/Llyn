# TEtymologyFace.cs
Hash: `c9beed6df8dc3c9f`

## `public sealed class TEtymologyFace`

Covers how the etymology field paints the verdicts its owning driver hands it.
The field lives on its own STA thread, since a WPF control demands one.

## `public void EtymologySourceShow_HandedVerdicts_PaintTheRowAndTheReadFace(`

The editor's fixed verdicts show the row of links and keep the read narrative hidden over text.
The lectern's verdicts show the read narrative with words and hide a row with no link.
The field paints only what it is handed, so a linked read side shows its row.
