# LGalleyDestination.cs
Hash: `1fa0aa199dab0f0a`

## `public sealed record LGalleyDestination(string? LGalleyDestinationName, bool LGalleyDestinationDefault, bool LGalleyDestinationChosen)`

One place a print can go, as the galley view lists it.
It carries facts only, so the gate chooses the wording it shows.

**Parameters**

- `LGalleyDestinationName`: the printer's queue name, or null for Save as PDF.
- `LGalleyDestinationDefault`: whether the printer is the system default.
- `LGalleyDestinationChosen`: whether the setting prints here.
