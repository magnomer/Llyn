# LMarkupReference.cs

## `public sealed record LMarkupReference(`

A bibliographic source as a markup file carries it, by value and with no id.
Authors are names in order rather than author references.
On import a stored reference with the same normalized title and year wins over these values.

**Parameters**

- `LMarkupReferenceTitle` — The title and what is known about it.
- `LMarkupReferenceYear` — The year and what is known about it.
- `LMarkupReferenceKind` — What the material is, unspecified when the file says nothing.
- `LMarkupReferenceUrl` — The address and what is known about it.
- `LMarkupReferenceNote` — Free text for what the named fields cannot say.
- `LMarkupReferenceAuthor` — Author names in file order.

## `public bool Equals(LMarkupReference? other)`

Field-by-field equality, authors compared in order.

## `public override int GetHashCode()`

A hash over every field and the author count.
