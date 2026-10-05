# CFrequency.cs
Hash: `fc6d2a52dde7ba9e`

## `public sealed record CFrequency(int CFrequencyBand, string CFrequencySource, CFrequencyTier CFrequencyRank, int CFrequencySpare, bool CFrequencyRanked)`

The frequency of an entry, as the frequency chip shows it.
A missing frequency is a null shape, so the chip hides.
Conduct raises a negative band or spare count to zero, since a chip cannot draw fewer than no stars.

**Parameters**

- `CFrequencyBand`: the band the chip stars, zero when no source ranks the entry.
- `CFrequencySource`: every source's figure, one per line, as the chip's tooltip.
- `CFrequencyRank`: the tier of the band, or the unknown tier at zero or for a name Conduct does not know.
- `CFrequencySpare`: the stars past the band in a full row.
- `CFrequencyRanked`: whether the entry's band lies inside the frequency scale, so the chip shows a star row.

## `public string CFrequencyKey`

The localization key of the chip's name, one per tier.
Conduct chose it, so a driver only looks it up.
Every tier has a key, so the chip never shows a missing name.
