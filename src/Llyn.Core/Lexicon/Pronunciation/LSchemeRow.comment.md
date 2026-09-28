# LSchemeRow.cs

## `public sealed record LSchemeRow(string LSchemeRowName, bool LSchemeRowTaken);`

One scheme a transcription row may be switched to, marked for that row's dropdown.

**Parameters**

- `LSchemeRowName` — The scheme as the language pack declares it.
- `LSchemeRowTaken` — True when another transcription row of the same draft already holds the scheme.
