# LCapsuleContent.cs
Hash: `f8a00a21a8ac6543`

## `public sealed record LCapsuleContent([property: JsonPropertyName("window")] LCapsuleWindow? LCapsuleContentWindow = null, [property: JsonPropertyName("linked")] bool LCapsuleContentLinked = true, [property: JsonPropertyName("columns")] IReadOnlyList<LCapsuleColumn>? LCapsuleContentColumn = null, [property: JsonPropertyName("press")] LCapsulePress? LCapsuleContentPress = null)`

The GUI-only state one workspace keeps between runs.
The stored keys are a data contract, so they stay lowercase and independent of member names.

**Parameters**

- `LCapsuleContentWindow`: the main window's geometry as last stored, or nothing before its first store.
- `LCapsuleContentLinked`: whether dragging a panel in one tab sets the same width in every tab, on by default.
- `LCapsuleContentColumn`: the stored panel widths, one record per tab, or nothing yet.
- `LCapsuleContentPress`: the print window's remembered size, or nothing yet.
