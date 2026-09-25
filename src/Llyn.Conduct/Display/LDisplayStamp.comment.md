# LDisplayStamp.cs

## `public sealed record LDisplayStamp(bool LDisplayStampShown, string LDisplayStampAdded, string LDisplayStampUpdated);`

The stamp row of the reading view, already worded, so the deportment only draws it.

**Parameters**

- `LDisplayStampShown`: whether the stored entry was read, which shows the row.
- `LDisplayStampAdded`: the creation time in local short form.
- `LDisplayStampUpdated`: the last update time in local short form.
