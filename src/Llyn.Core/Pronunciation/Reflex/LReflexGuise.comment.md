# LReflexGuise.cs

## `public sealed record LReflexGuise(bool LReflexGuiseRespelled, bool LReflexGuisePhonemic, bool LReflexGuiseFolded);`

How one reflex row prints, as the engine answers it for the row's language.

**Parameters**

- `LReflexGuiseRespelled` — True when the row's language shows its respelling under the current switch.
- `LReflexGuisePhonemic` — True when the row's language writes its readings as phonemes.
- `LReflexGuiseFolded` — True when the pack of the entry's language folds the row's language away.
