# CScreen.cs
Hash: `060eb21146468883`

## `public sealed record CScreen(Uri CScreenAddress, string? CScreenFilm)`

What a video screen plays, as the engine read it from a stored location.

**Parameters**

- `CScreenAddress`: the resolved address, a file on this machine or a web address.
- `CScreenFilm`: the hosted film id inside a web address, or null for any other address.
