# QMentionChip.cs
Hash: `cc27f95b02746bc8`

## `internal static class QMentionChip`

The driver map from a Conduct Mention label to the chip a chip line shows.
It keeps the Conduct record out of the line, so the line only diffs chips.

## `internal static IReadOnlyList<PMentionChip> QMentionChipCreate(IReadOnlyList<CMentionLabel> labels)`

The chips of a line, one per ready label, in Mention order.
A label carrying a key shows the key's text as its name.
Conduct chose that key for a Mention standing for nothing.
