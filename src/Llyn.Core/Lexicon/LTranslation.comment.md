# LTranslation.cs

## `public sealed record LTranslation(`

One translation link a Meaning or Collocation carries to another Entry.
The link stores the target Entry's id and never its text.
So renaming a headword changes nothing about the link that points at it.
Links are held in order on the card that carries them.

**Parameters**

- `LTranslationEntryId` — Id of the Entry this link points at.
- `LTranslationPosition` — Order among the owning card's translations.
