# CSentenceDraft.cs
Hash: `705f990e2d9ef2f8`

## `public sealed record CSentenceDraft(`

One sentence of a card, as the sentence row shows it.

**Parameters**

- `CSentenceDraftId`: the stored sentence, zero for a fresh one.
- `CSentenceDraftExample`: the example, or null when the sentence holds none yet.
- `CSentenceDraftCited`: true when the example cites a reference, so the idle list keeps the citation shown.
- `CSentenceDraftText`: the example's text, worded with the example hint, and empty while no example is held.
- `CSentenceDraftParticle`: the particle framing the example, worded with its hint.
- `CSentenceDraftDependence`: the dependence framing the example, worded with its hint.
