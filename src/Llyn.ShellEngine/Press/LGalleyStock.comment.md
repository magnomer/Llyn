# LGalleyStock.cs
Hash: `11fcb031a5ccf774`

## `public sealed record LGalleyStock(string? LGalleyStockName, double LGalleyStockWidth, double LGalleyStockHeight, bool LGalleyStockChosen)`

One sheet the chosen destination offers, as the galley view lists it.
It carries facts only, so the gate chooses the wording it shows.

**Parameters**

- `LGalleyStockName`: the sheet's standard name, or null for an unnamed form.
- `LGalleyStockWidth`: the upright width in inches.
- `LGalleyStockHeight`: the upright height in inches.
- `LGalleyStockChosen`: whether the setting uses this sheet.
