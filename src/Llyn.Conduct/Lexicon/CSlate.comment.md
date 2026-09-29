# CSlate.cs

## `public sealed record CSlate(`

What a tag field keeps after a gate, and the dropdown of stored Tags it offers, ready to show.
The driver paints it as it comes and decides nothing about the rows.

**Parameters**

- `CSlateText`: the text the entry keeps once the completed tags went onto the card.
- `CSlateRows`: the stored Tags the kept text matches, in the order the engine offers them.
- `CSlateShown`: the engine's verdict that the dropdown opens.
