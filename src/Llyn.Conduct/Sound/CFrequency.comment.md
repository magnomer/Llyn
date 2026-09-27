# CFrequency.cs

## `public sealed record CFrequency(int CFrequencyBand, string CFrequencySource)`

The frequency of an entry, as the frequency chip shows it.
A missing frequency is a null shape, so the chip hides.

**Parameters**

- `CFrequencyBand`: the band the chip stars, zero when no source ranks the entry.
- `CFrequencySource`: every source's figure, one per line, as the chip's tooltip.
