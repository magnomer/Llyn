# TCorpusScribe.cs
Hash: `8ecc1fd49b2fdcb7`

## `public sealed class TCorpusScribe`

Covers the corpus transcript desk, how it opens and how it is left, on a real workspace.
It builds each corpus through `TCorpus.TCorpusPrepare`.
A fresh start with no row chosen opens a blank transcript under a checked scribe.
A click away from an unsaved transcript that is kept records nothing.
A leave with nothing unsaved asks nothing, and a discard stores nothing.
A kept leave stays on the transcript, and a stored leave keeps the Example.
A saved fresh transcript shows the stored Example on the excerpt outside the scribe.
Closing the transcript cancels the desk, and closing the quotation editor falls back to the chosen Example.
An entry notice with no chosen quotation keeps the transcript held.
