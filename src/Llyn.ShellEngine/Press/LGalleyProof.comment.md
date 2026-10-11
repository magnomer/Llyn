# LGalleyProof.cs
Hash: `ec058355dde52ee4`

## `public sealed record LGalleyProof(int LGalleyProofCount, double LGalleyProofWidth, double LGalleyProofHeight, int LGalleyProofResolution)`

The relay's answer to a proof draw that was not superseded.
The canvas lays its sheets out from it before any page is drawn.

**Parameters**

- `LGalleyProofCount`: how many pages the proof has.
- `LGalleyProofWidth`: the first page's width in device-independent pixels, 96 to the inch.
- `LGalleyProofHeight`: the first page's height in device-independent pixels, 96 to the inch.
- `LGalleyProofResolution`: the dots per inch a page is drawn at when no resolution is asked.
  Conduct's lens asks for a higher one only when the zoom needs more detail than this.
