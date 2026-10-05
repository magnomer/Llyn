# TTranscriptionScheme.cs
Hash: `b3977faf30f7a61f`

## `public sealed class TTranscriptionScheme`

Covers the scheme choices a transcription row offers in its combo box.
The row is a plain notifying object, so the case needs no STA thread.
No application runs, so each choice label falls back to its scheme name.

## `public void TranscriptionSchemeRefine_Reordered_PairsByName()`

The same two schemes arrive in a new order with the same count.
The choices are rebuilt in the new order, and each taken flag lands on its own scheme.
A count check alone kept the old order and handed each flag to the wrong scheme.
A later refresh with the same schemes keeps the choice objects and only moves the taken flags.
So a bound combo box keeps its items when nothing but the flags changed.
