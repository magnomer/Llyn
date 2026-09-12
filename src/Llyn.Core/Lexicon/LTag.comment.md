# LTag.cs

## `public sealed record LTag(`

One Tag.
A Tag is a shared row any number of Meanings and Collocations link to by id.
`LTagId` is the identity, an opaque and stable id.
The text is visible data and never identity.
Renaming it leaves the id and every link to it untouched.
Two Tags never read alike, because the store keeps the text unique.
A Tag carries no state of its own, and a nameless Tag does not exist.
The order a Tag appears in lives on the card that carries it rather than here.
So the same Tag can sit first under one Meaning and third under a Collocation.

**Parameters**

- `LTagId` — Opaque stable id of the stored row.
- `LTagText` — The tag text, display text and never identity.
  Text may hold spaces and ordinary punctuation.
  Only a comma separates one Tag from the next where Tags are written as a line.
