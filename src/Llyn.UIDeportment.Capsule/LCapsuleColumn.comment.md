# LCapsuleColumn.cs
Hash: `44bd0b0b1df9a41e`

## `public sealed record LCapsuleColumn([property: JsonPropertyName("tab")] string LCapsuleColumnTab, [property: JsonPropertyName("left")] double? LCapsuleColumnLeft, [property: JsonPropertyName("middle")] double? LCapsuleColumnMiddle)`

The widths of one tab's fixed panels.
The last column takes whatever room the window leaves, so it is never stored.

**Parameters**

- `LCapsuleColumnTab` — The lowercase name of the tab, for example `"library"`.
- `LCapsuleColumnLeft` — The width of the leftmost panel, or nothing before it was ever dragged.
- `LCapsuleColumnMiddle` — The width of the middle panel where a tab has three, or nothing otherwise.
