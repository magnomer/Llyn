# CMeaning.cs
Hash: `dd7aa1487d775214`

## `public sealed record CMeaning(long CMeaningId, string CMeaningName, int CMeaningDepth, string CMeaningPath = "");`

One Meaning as the sense menu, the link picker or a collocation's link chip shows it.
The menu and the picker rows come in reading order, the chips in the card's link order.

**Parameters**

- `CMeaningId`: the Meaning the row picks.
- `CMeaningName`: the title, the definition, or the unknown label.
- `CMeaningDepth`: how deep the Meaning sits under its parents, zero on a link chip.
- `CMeaningPath`: its outline number under its parents, such as `2.1`, so a link chip names where the Meaning sits.
  It is empty by default.
