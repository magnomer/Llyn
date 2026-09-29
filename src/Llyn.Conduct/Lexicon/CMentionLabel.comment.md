# CMentionLabel.cs

## `public sealed record CMentionLabel(`

One chip of a mention line, labelled by the engine.

**Parameters**

- `CMentionLabelId`: the Mention the chip stands for.
- `CMentionLabelWord`: the words the Mention covers.
- `CMentionLabelName`: the linked headword, or the silent label when unlinked.
- `CMentionLabelSense`: the linked sense's name, empty when none.
- `CMentionLabelLinked`: whether the Mention stands for an Entry.

## `public string? CMentionLabelKey`

The key a driver looks up in place of the name, chosen for a Mention that stands for nothing.
A linked chip has none, so its headword shows as the engine named it.
