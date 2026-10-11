# CSPressChoice.cs
Hash: `babf390eb082c58b`

## `public sealed record CSPressChoice<CSPressChoiceKind>(CSPressChoiceKind CSPressChoiceValue, string CSPressChoiceKey, bool CSPressChoiceChosen)`

One row of a print window picker, ready to show.
The setting answers each picker as a list of these rows in display order.
The driver fills the picker from that list alone and hands back the raw row number.
The index gate maps that number through the same list, so no order is shared in secret.

**Parameters**

- `CSPressChoiceValue`: the value the row stands for, which the index gate applies.
- `CSPressChoiceKey`: the wording key the picker shows for the row.
- `CSPressChoiceChosen`: whether this row is the one the setting uses.
