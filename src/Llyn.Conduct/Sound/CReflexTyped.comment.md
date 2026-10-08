# CReflexTyped.cs
Hash: `5ebe59ff82f5b3d2`

## `public sealed record CReflexTyped(CReflex? CReflexTypedRow, IReadOnlyList<CReflexHead> CReflexTypedHeads)`

The answer to one typed reflex cell, as `CKindred.CKindredSet` gives it.
A driver writes its row from this answer alone, never from what the user typed.

**Parameters**

- `CReflexTypedRow`: the row as it now reads, built by `CReflex.CReflexTypedApply`.
  It holds the typed text when the gate took it, else the text the draft holds.
  It is null for a row the held draft lacks.
- `CReflexTypedHeads`: every row's lead while a language is typed, else empty.
