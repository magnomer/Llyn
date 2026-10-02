# CLecternAccent.cs
Hash: `92ed38553d3d9914`

## `public sealed record CLecternAccent(`

The pronunciation block of the reading view for the shown entry, ready to show.
The display reads it whenever an entry opens, so the driver only draws it.

**Parameters**

- `CLecternAccentMark`: the respelling switch and the brackets around every reading.
- `CLecternAccentContour`: the primary reading's contour syllables, empty when the contour hides.
- `CLecternAccentText`: the primary reading as the switch shows it, empty when there is none.
- `CLecternAccentSpoken`: whether the primary reading holds text, which shows its surface.
- `CLecternAccentPrimary`: the primary pronunciation's variety, with its label and flag keys.
- `CLecternAccentRows`: the further pronunciations, one row each, in the entry's order.
- `CLecternAccentFlagged`: whether the pack draws varieties as flags rather than names.
