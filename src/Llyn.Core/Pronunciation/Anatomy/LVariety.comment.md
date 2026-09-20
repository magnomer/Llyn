# LVariety.cs

## `public sealed record LVariety(string LVarietyName, string? LVarietyFlag)`

One regional variety a language pack declares, such as British or American.
A reading names its variety by this name, and the UI draws it by this flag.
The engine holds no list of varieties of its own.

**Parameters**

- `LVarietyName` — The variety's name as the pack spells it, the tag a reading carries.
- `LVarietyFlag` — The flag as an ISO 3166-1 alpha-2 country code (for example `us`).
  It is `null` when the pack declares none, and the name is shown as text instead.
