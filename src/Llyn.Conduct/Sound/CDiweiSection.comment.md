# CDiweiSection.cs
Hash: `758ca18d8d4174ac`

## `public sealed record CDiweiSection(string CDiweiSectionLabel, IReadOnlyList<CDiweiLine> CDiweiSectionLines, IReadOnlyList<CTally> CDiweiSectionTallies, bool CDiweiSectionSwitched, bool CDiweiSectionRespelled)`

One section of a diwei page, a division or a place with its lines and tallies.

**Parameters**

- `CDiweiSectionLabel`: the localized heading the section prints.
- `CDiweiSectionLines`: the lines of the section, already sorted by the engine.
- `CDiweiSectionTallies`: the reflex tallies of the section, in the chosen notation.
- `CDiweiSectionSwitched`: whether the notation switch shows.
- `CDiweiSectionRespelled`: whether respelling rather than IPA is chosen.
