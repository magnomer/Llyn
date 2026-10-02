# CDiweiLine.cs
Hash: `0b229f53dcd37ff6`

## `public sealed record CDiweiLine(`

One line of a diwei section: a reading, its label and its characters.

**Parameters**

- `CDiweiLineReading`: the reconstructed reading between slashes, or empty.
- `CDiweiLineLabel`: the initial or rime the line names.
- `CDiweiLineRounded`: whether the rime line is rounded.
- `CDiweiLineCharacters`: the characters of the line, in first-seen order.
