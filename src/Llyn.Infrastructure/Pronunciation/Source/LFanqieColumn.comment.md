# LFanqieColumn.cs
Hash: `fe092d15c9a6224c`

## `internal sealed record LFanqieColumn(string LFanqieColumnText, string LFanqieColumnDivision, string LFanqieColumnTone)`

One column heading of a rime table, as printed and as read apart.

**Parameters**

- `LFanqieColumnText` — The heading as text, such as `一等平`, appended to every line under it.
- `LFanqieColumnDivision` — The division the column pattern read from it, or empty without the pattern.
- `LFanqieColumnTone` — The tone the column pattern read from it, or empty without the pattern.
