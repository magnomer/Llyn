# CScenarioLine.cs
Hash: `e72ee0d291086452`

## `public sealed record CScenarioLine(string CScenarioLineText, string CScenarioLineHint)`

One field of the repertoire's edit area, as its driver fills the field and its unseen twins.

**Parameters**

- `CScenarioLineText`: the field's text, empty when nothing legible is written.
- `CScenarioLineHint`: the key of the field's placeholder, which Conduct chooses and the driver looks up.

## `public bool CScenarioLineVacant`

Whether the field is empty, so its placeholder shows.

## `public string? CScenarioLineWording`

The key a measuring twin words instead of the text, while the field is empty.
Null while the text itself shows.
