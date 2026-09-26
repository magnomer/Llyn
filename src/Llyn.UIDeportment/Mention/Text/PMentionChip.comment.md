# PMentionChip.cs

## `internal sealed record PMentionChip(`

One Mention as the chip line shows it under an editor row.
It is display only and never edits the span.
Editing is the gesture on the text.

**Parameters**

- `PMentionChipId`: the Mention's id, which the remove button names in its request.
- `PMentionChipWord`: the span cut from the current sentence text.
- `PMentionChipName`: the headword the word stands for, or the silent mark when it stands for nothing.
- `PMentionChipSense`: the title of the Meaning it is narrowed to, or empty for the whole Entry.

## `internal static void PMentionChipApply(FrameworkElement container, object item, string? _)`

Fills one chip of `Theme.Mention.Chip`: the word, the headword, the sense and the remove button.
The button takes the unlink command with the chip as its parameter, and its cross icon.
Every chip line attaches it through `PLookItem.PLookItemAttach`.
