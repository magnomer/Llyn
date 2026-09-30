# CMentionSense.cs

## `public sealed record CMentionSense(string CMentionSenseKey, IReadOnlyList<CMeaning> CMentionSenseRow);`

The sense menu of a linked Mention, ready to show.

**Parameters**

- `CMentionSenseKey`: the localization key of the menu's title, which the driver looks up.
- `CMentionSenseRow`: the rows in the order shown, the whole Entry first with id zero.
  The Meanings follow in reading order.
