# CNotationItem.cs
Hash: `4238658664f1d524`

## `public sealed record CNotationItem(`

One source row of the notation popup, as the errand keeps it.
A row appears the moment its source starts searching, so every declared source shows before any answers.
It is replaced in place as its source answers, so a row never jumps.

**Parameters**

- `CNotationItemSource`: the name of the source the row stands for.
- `CNotationItemOrder`: the source's place in the pack, which keeps the row where the pack put it.
- `CNotationItemReading`: the readings the source gave so far, in the order they arrived.
- `CNotationItemNotice`: the notice key a row without a reading shows, empty once one landed.
- `CNotationItemReady`: whether the row carries a reading the user can take now.
