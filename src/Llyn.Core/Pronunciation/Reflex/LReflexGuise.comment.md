# LReflexGuise.cs
Hash: `c886a7cc91060e18`

## `public sealed record LReflexGuise(bool LReflexGuiseRespelled, bool LReflexGuisePhonemic, bool LReflexGuiseFolded, string LReflexGuiseShown = "")`

How one reflex row prints, as the engine answers it for the row's language.

**Parameters**

- `LReflexGuiseRespelled` — True when the row's language shows its respelling under the current switch.
- `LReflexGuisePhonemic` — True when the row's language writes its readings as phonemes.
- `LReflexGuiseFolded` — True when the pack of the entry's language folds the row's language away.
- `LReflexGuiseShown` — The text the row shows, filled once by the engine through `LReflexGuiseRead`.

## `public static LReflexGuise? LReflexGuiseFind(IReadOnlyList<LReflexGuise> guises, int index)`

The guise standing at `index` of `guises`, which run by position beside their reflex rows.
An index past the list, or below zero, answers null, so a row without a guise prints plain.
The Outpost entry sheet and the series members read their guises through it.

## `public string LReflexGuiseRead(string phonetic, string? respelling)`

The form a reflex row shows, the one owner of the respelling choice.
It is the respelling while the guise is respelled and one is filled, and `phonetic` otherwise.
`LLanguageClerk.LLanguageGuiseScan` applies it once per row and keeps the answer in `LReflexGuiseShown`.
