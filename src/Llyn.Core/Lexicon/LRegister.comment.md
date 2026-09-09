# LRegister.cs

## `public sealed record LRegister(`

One Register — the formality or politeness level a Meaning or Collocation is marked with.
It is independent data owned by nothing, the way a Situation is.
Any number of Meanings and Collocations *reference* it instead of containing it.
The order a Register appears in lives on each reference rather than here.
`LRegisterId` is the identity, an opaque and stable id.
The name is visible data and never identity.
Renaming it leaves the id and every reference to it untouched.
A built-in Register is one a language pack ships, and its id names the language it came from.
A written Register is one the user typed, and it carries no language.
The name carries `LStateValue`, so a name standing empty says whether nothing was ever recorded.
It says instead when something was recorded that cannot be read back.

**Parameters**

- `LRegisterId` — Opaque stable id, generated for a written Register and fixed for a built-in one.
- `LRegisterName` — The register name and what is known about it, display text and never identity.
- `LRegisterLanguage` — The language pack a built-in Register came from, empty for a written one.
- `LRegisterBuiltin` — Whether a language pack ships this Register rather than the user writing it.
