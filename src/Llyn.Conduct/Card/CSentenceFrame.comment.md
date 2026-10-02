# CSentenceFrame.cs
Hash: `dcb9e108a9b63eb9`

## `public sealed record CSentenceFrame(`

The frame of the sentence rows in one language, ready for a driver to show.

**Parameters**

- `CSentenceFrameOrder`: the word order that places the particle before or after the dependence.
- `CSentenceFrameParticle`: the particles a row may pick, empty when none are known.
- `CSentenceFrameDependence`: the dependences a row may pick, empty when none are known.
