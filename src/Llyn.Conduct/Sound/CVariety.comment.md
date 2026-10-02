# CVariety.cs
Hash: `ee5d46761a6ec6a8`

## `public sealed record CVariety(string CVarietyName, string CVarietyKey, string CVarietyEnsign)`

One variety of a reading, as a driver labels it or flags it.
Conduct chose both keys, so a driver only looks them up.

**Parameters**

- `CVarietyName`: the variety's raw name, empty for the main reading.
- `CVarietyKey`: the localization key of the variety's label.
- `CVarietyEnsign`: the key the variety's flag is kept under, empty when the variety has no flag.
