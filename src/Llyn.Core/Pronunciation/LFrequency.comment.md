# LFrequency.cs

## `public sealed record LFrequency(string LFrequencySource, string LFrequencyRaw, string? LFrequencyBand)`

One frequency value an entry carries.
The engine fetches it once from a web source the language pack names.
The database keeps only the fetched value for that entry, never a frequency list.
The pack, not the engine, knows where the value lives and what label it earns.

**Parameters**

- `LFrequencySource` — The name of the pack source the raw figure came from.
- `LFrequencyRaw` — The figure exactly as the source returned it, such as a rank or a count.
- `LFrequencyBand` — The label the pack's bands map the raw figure to, held as an [LBand](LBand.comment.md) name.
  It is `null` when no band matches or when the value was read back from storage unresolved.

## `public bool LFrequencyNumeric`

Whether the raw figure reads as a decimal in the invariant culture.
A figure that does is a count or a rank a unit can follow, and a code such as a list mark is not.
The record decides this once, so no surface reading it has to parse the figure itself.

## `public bool LFrequencyNumeric`

Whether the raw figure reads as a decimal in the invariant culture.
A figure that does is a count or a rank a unit can follow, and a code such as a list mark is not.
The record decides this once, so no surface reading it has to parse the figure itself.

## `public static LFrequency LFrequencyParse(string stored)`

Parses a stored `"<source>|<raw>"` string back into a record.
The split happens on the first `|` only, so a raw figure may itself carry the character.
A stored string without `|` is read as a raw figure with an empty source.
The band is not stored, so the parsed record carries `null` and the engine resolves it again.

## `public string LFrequencyFormat()`

Produces the `"<source>|<raw>"` string the database keeps on the entry.
