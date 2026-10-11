# CSPressPage.cs
Hash: `072a1a25bb06b306`

## `public sealed record CSPressPage(int CSPressPageIndex, byte[]? CSPressPagePng)`

One change to a page of the proof, either a drawn picture or a drop.
A single record for both keeps one handler as the only writer of the driver's page cache.
The driver decodes it at the shown width and grays it only for a gray print.
It carries no size, since the canvas sizes each sheet from the paper in `CSPressState`.
The resolution it was drawn at stays with `CSGalley`, which tracks delivered pages by resolution.

**Parameters**

- `CSPressPageIndex`: the page the image shows, counted from zero.
- `CSPressPagePng`: the page as PNG bytes, or null when the page left the delivery window.
  A dropped page frees its picture, so a later visit asks for it again.
