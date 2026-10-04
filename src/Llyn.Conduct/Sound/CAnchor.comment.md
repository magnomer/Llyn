# CAnchor.cs
Hash: `35297b8cb4f7d0b6`

## `public sealed record CAnchor(IReadOnlyList<CAnchorRow> CAnchorRows, bool CAnchorEmpty)`

The anchor menu of one reflex row, ready to show.

**Parameters**

- `CAnchorRows`: the fanqie readings the row may anchor to, in the engine's order.
- `CAnchorEmpty`: whether the menu shows its notice in place of rows.
