# LMarkupSentence.cs

## `public sealed record LMarkupSentence(`

One card's hold on one example as a markup file carries it.
The frame belongs to the card and the example travels inside the row, since the file shares nothing by id.
A row may state a frame and no example at all.

**Parameters**

- `LMarkupSentenceExample` — The example the row quotes, and `null` when the row states a frame alone.
- `LMarkupSentenceParticle` — The frame's grammatical marker, and what is known about it.
- `LMarkupSentenceDependence` — The role the frame fills, and what is known about it.
