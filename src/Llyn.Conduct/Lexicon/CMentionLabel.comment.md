# CMentionLabel.cs

## `public sealed record CMentionLabel(`

One chip of a mention line, labelled by the engine.

**Parameters**

- `CMentionLabelId`: the Mention the chip stands for.
- `CMentionLabelWord`: the words the Mention covers.
- `CMentionLabelName`: the linked headword, or the silent label when unlinked.
- `CMentionLabelSense`: the linked sense's name, empty when none.
