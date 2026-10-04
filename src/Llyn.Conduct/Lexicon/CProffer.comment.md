# CProffer.cs
Hash: `8b6860f7cafb1ed0`

## `public sealed record CProffer(string CProfferText, IReadOnlyList<CProfferRow> CProfferRows, bool CProfferShown)`

What a card field keeps after a gate, and the dropdown of stored rows it offers, ready to show.
One shape serves every field the shared dropdown opens for.
The corpus transcript's citation drawer shows the same shape, since it offers the same citation find.
The driver paints it as it comes and decides nothing about the rows.

**Parameters**

- `CProfferText`: the text the entry keeps once the completed parts went onto the card.
  A citation field writes nothing, so its text comes back unchanged and is not painted.
- `CProfferRows`: the stored rows the kept text matches, in the order the engine offers them.
- `CProfferShown`: the engine's verdict that the dropdown opens.
