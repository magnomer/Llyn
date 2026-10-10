# LReflexGuise.cs
Hash: `9ec772e32203354a`

## `public sealed record LReflexGuise(bool LReflexGuiseRespelled, bool LReflexGuisePhonemic, bool LReflexGuiseFolded)`

How one reflex row prints, as the engine answers it for the row's language.

**Parameters**

- `LReflexGuiseRespelled` — True when the row's language shows its respelling under the current switch.
- `LReflexGuisePhonemic` — True when the row's language writes its readings as phonemes.
- `LReflexGuiseFolded` — True when the pack of the entry's language folds the row's language away.

## `public static LReflexGuise? LReflexGuiseFind(IReadOnlyList<LReflexGuise> guises, int index)`

The guise standing at `index` of `guises`, which run by position beside their reflex rows.
An index past the list, or below zero, answers null, so a row without a guise prints plain.
The Outpost entry sheet and the series members read their guises through it.
