# CMarker.cs
Hash: `5593c710aee3c7f3`

## `public sealed record CMarker(IReadOnlyList<string> CMarkerSpeeches, string CMarkerTyped, CCategory CMarkerCategory, IReadOnlyList<(string, bool)> CMarkerUnits, IReadOnlyList<(string, bool)> CMarkerFreedoms);`

The part of speech field as the editor shows it.
The lexical unit and freedom droppers head its row.

**Parameters**

- `CMarkerSpeeches`: the committed parts, one chip each by its shown name.
- `CMarkerTyped`: the pending text the field keeps.
- `CMarkerCategory`: the category menu for the pending text and the chips.
- `CMarkerUnits`: the units every draft is offered, each as its localization key and whether the draft holds it.
  They are plain pairs, so the unit menu adds no type to what Conduct offers its drivers.
- `CMarkerFreedoms`: the freedoms every draft is offered, paired the same way.
