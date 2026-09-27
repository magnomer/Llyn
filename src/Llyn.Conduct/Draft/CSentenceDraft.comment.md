# CSentenceDraft.cs

## `public sealed record CSentenceDraft(`

One sentence of a card, as the sentence row shows it.

**Parameters**

- `CSentenceDraftId`: the stored sentence, zero for a fresh one.
- `CSentenceDraftExample`: the example, or null when the sentence holds none yet.
- `CSentenceDraftParticle`: the particle framing the example.
- `CSentenceDraftDependence`: the dependence framing the example.
