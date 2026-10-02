# CMarker.cs
Hash: `fb49f03fb9bbcace`

## `public sealed record CMarker(IReadOnlyList<string> CMarkerSpeeches, string CMarkerTyped, CCategory CMarkerCategory);`

The part of speech field as the editor shows it.

**Parameters**

- `CMarkerSpeeches`: the committed parts, one chip each by its shown name.
- `CMarkerTyped`: the pending text the field keeps.
- `CMarkerCategory`: the category menu for the pending text and the chips.
