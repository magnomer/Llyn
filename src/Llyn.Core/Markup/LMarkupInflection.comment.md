# LMarkupInflection.cs

## `public sealed record LMarkupInflection(`

An inflected form as a markup file carries it.
Its speech and morphologies are names, since the file stores no ids.

**Parameters**

- `LMarkupInflectionText` — The inflected form.
- `LMarkupInflectionLocal` — Its local spelling, `null` when none is given.
- `LMarkupInflectionSpeech` — The part of speech by name, empty when none is given.
- `LMarkupInflectionMorphology` — Grammatical features by name, in file order.

## `public bool Equals(LMarkupInflection? other)`

Field-by-field equality, morphologies compared in order.

## `public override int GetHashCode()`

A hash over the text fields and the morphology count.
