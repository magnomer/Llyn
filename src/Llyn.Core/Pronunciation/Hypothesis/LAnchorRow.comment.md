# LAnchorRow.cs

## `public sealed record LAnchorRow(LFanqieRow LAnchorRowFanqie, bool LAnchorRowHeld, bool LAnchorRowEstimated);`

One stored fanqie row of a character, marked for the anchor dropdown of one reflex row.

**Parameters**

- `LAnchorRowFanqie` — The stored fanqie row the mark stands for.
- `LAnchorRowHeld` — True when the reflex row anchors this fanqie row.
- `LAnchorRowEstimated` — True when the fanqie row carries a tone class the estimate reads.
