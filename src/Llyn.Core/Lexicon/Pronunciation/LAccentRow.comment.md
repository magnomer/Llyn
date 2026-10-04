# LAccentRow.cs
Hash: `f807d8eedeac364a`

## `public sealed record LAccentRow(long LAccentRowId, string LAccentRowVariety, string LAccentRowText, string LAccentRowAudio)`

One pronunciation of the shown entry as the reading view prints it.
The engine answers it with the reading already resolved under the respelling switch.

**Parameters**

- `LAccentRowId` — Id of the stored pronunciation row, or zero when the entry has none.
- `LAccentRowVariety` — The variety label, or empty when the pronunciation names none.
- `LAccentRowText` — The respelling when the switch shows one, else the phonetic reading.
- `LAccentRowAudio` — Path to the recording, or empty when none was chosen.
