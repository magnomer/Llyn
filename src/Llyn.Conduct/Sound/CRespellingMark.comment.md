# CRespellingMark.cs
Hash: `0e039bb6dba672d3`

## `public sealed record CRespellingMark(`

How one language's readings print, as `CRespelling` decided it.
A driver compares two marks to know whether a row must be rebuilt.

**Parameters**

- `CRespellingMarkShown`: whether the respelling is printed when one is filled.
- `CRespellingMarkOpener`: the text drawn before the reading, possibly empty.
  A slash here marks a phonemic reading, so two marks differ whenever phonemic status does.
- `CRespellingMarkCloser`: the text drawn after the reading, possibly empty.
