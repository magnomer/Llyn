# CFrequency.cs
Hash: `73ccde2d55b7f548`

## `public sealed record CFrequency(int CFrequencyBand, string CFrequencySource, string CFrequencyRank, int CFrequencySpare, bool CFrequencyRanked)`

The frequency of an entry, as the frequency chip shows it.
A missing frequency is a null shape, so the chip hides.

**Parameters**

- `CFrequencyBand`: the band the chip stars, zero when no source ranks the entry.
- `CFrequencySource`: every source's figure, one per line, as the chip's tooltip.
- `CFrequencyRank`: the ladder name of the band, or the unknown name at zero.
- `CFrequencySpare`: the stars past the band in a full row.
- `CFrequencyRanked`: whether any source ranks the entry, so the chip shows a star row.

## `public string CFrequencyKey`

The localization key of the chip's name.
Conduct chose it, so a driver only looks it up.
