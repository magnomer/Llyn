# LEnsignRow.cs
Hash: `3c89e4a238b67f9b`

## `public sealed record LEnsignRow(string LEnsignRowKey, string LEnsignRowPath)`

One flag the cache kept.
It pairs the key it was asked under with the SVG path on disk.
The veneer draws the path and stores the drawing under the key.
