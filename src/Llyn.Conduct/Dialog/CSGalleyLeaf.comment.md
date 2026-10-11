# CSGalleyLeaf.cs
Hash: `41ecbf1e5422afda`

## `internal sealed record CSGalleyLeaf(int CSGalleyLeafPage, int? CSGalleyLeafResolution)`

One page picture the galley delivered, keyed by page and resolution.
A record gives value equality, so a set finds a picture by its two values.

**Parameters**

- `CSGalleyLeafPage`: the page index in the current proof, counted from zero.
- `CSGalleyLeafResolution`: the dots per inch it was asked at, or null for the engine's default.
