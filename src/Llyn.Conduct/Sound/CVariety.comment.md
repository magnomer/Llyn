# CVariety.cs
Hash: `16ce6347974fd167`

## `public sealed record CVariety(string CVarietyName, string CVarietyKey, string CVarietyEnsign)`

One variety of a reading, as a driver labels it or flags it.
Conduct chose both keys, so a driver only looks them up.

**Parameters**

- `CVarietyName`: the variety's raw name, empty for the main reading.
- `CVarietyKey`: the localization key of the variety's label.
- `CVarietyEnsign`: the key the variety's flag is kept under, empty when the variety has no flag.

## `public static CVariety CVarietyRead(string language, string variety)`

A variety of the language, with the key of its label and the key of its flag.
The label key is `Variety.` and the name, which a driver looks up and falls back to the name.
The flag key comes from the engine, so the ensign's key format has one owner.
