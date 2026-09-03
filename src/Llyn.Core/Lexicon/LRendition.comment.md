# LRendition.cs

## `public sealed record LRendition(`

One rendition of an `LExample` into another language.
A rendition is owned text: it belongs to exactly one Example, is ordered within it, and is removed with it.
Its identity is `LRenditionId`, an opaque and program-generated stable id.
So reordering rewrites `LRenditionPosition` only and never changes which rendition is which.

**Parameters**

- `LRenditionId` — Opaque, program-generated stable id.
- `LRenditionLanguage` — Language code the rendition is written in.
- `LRenditionText` — The rendered text.
- `LRenditionPosition` — Order among the owning Example's renditions.
