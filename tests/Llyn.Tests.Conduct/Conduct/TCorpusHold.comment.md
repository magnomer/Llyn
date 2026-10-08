# TCorpusHold.cs
Hash: `d5ed98be21918e0f`

## `public sealed class TCorpusHold`

Covers the marshal the corpus is built with, the Example it holds and the exit gate, on a real workspace.
The marshal the corpus is built with carries each transcript edit to `CTranscriptDraftChanged` as the held Example.
The edit and the persist after it raise exactly one draft notice between them.
A held stored Example carries its tally and text placeholder, and a cancel raises the blank Example with both.
The window's exit gate cancels the entry editor's desk and stops its display's playback once.
