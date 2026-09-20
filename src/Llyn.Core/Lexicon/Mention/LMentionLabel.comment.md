# LMentionLabel.cs

## `public sealed record LMentionLabel(`

One Mention of a sentence as the engine hands it to a chip line, its words already read.
The line shows it and reads nothing more.

**Parameters**

- `LMentionLabelId` — The id of the Mention the chip stands for.
- `LMentionLabelWord` — The span of the sentence text the Mention occupies.
- `LMentionLabelEntry` — The Entry the Mention points at, zero when it stands for nothing.
- `LMentionLabelName` — The headword of that Entry, empty when the Mention stands for nothing.
- `LMentionLabelSense` — The title of the narrowed Meaning, its definition when untitled, empty when unnarrowed.
