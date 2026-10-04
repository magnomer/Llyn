# LCapsuleContent.cs
Hash: `317a1c8615f8b974`

## `public sealed record LCapsuleContent([property: JsonPropertyName("window")] LCapsuleWindow? LCapsuleContentWindow = null, [property: JsonPropertyName("linked")] bool LCapsuleContentLinked = true, [property: JsonPropertyName("columns")] IReadOnlyList<LCapsuleColumn>? LCapsuleContentColumn = null)`

The GUI-only state one workspace keeps between runs.
The stored keys are a data contract, so they stay lowercase and independent of member names.

**Parameters**

- `LCapsuleContentWindow` — The restored window rectangle from the last run, or nothing before a first close.
- `LCapsuleContentLinked` — Whether dragging a panel in one tab sets the same width in every tab, on by default.
- `LCapsuleContentColumn` — The stored panel widths, one record per tab, or nothing yet.
