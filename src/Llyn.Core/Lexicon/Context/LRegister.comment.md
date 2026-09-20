# LRegister.cs

## `public sealed record LRegister(`

One Register — the formality or politeness level a Meaning or Collocation is marked with.
It is independent data owned by nothing, the way a Situation is.
Any number of Meanings and Collocations *reference* it instead of containing it.
The order a Register appears in lives on each reference rather than here.
`LRegisterId` is the identity, an opaque and stable id.
The name is visible data and never identity.
Renaming it leaves the id and every reference to it untouched.
A built-in Register is one a language pack names.
A written Register is one the user typed.
A Register belongs to no language.
Every pack naming the same name names the same Register, and a card in any language marks it.
The name carries `LStateValue`, so a name standing empty says whether nothing was ever recorded.
It says instead when the user marked the value as not known.

**Parameters**

- `LRegisterId` — Opaque stable id, generated when the row is first stored.
- `LRegisterName` — The register name and what is known about it, display text and never identity.
- `LRegisterBuiltin` — Whether a language pack names this Register rather than the user writing it.
