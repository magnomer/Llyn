# PDisplayTranscription.cs

## `public partial class PDisplay`

The transcriptions of a shown entry, listed under its pronunciations.
Each stands with its scheme as a chip and its text beside it.
There is nothing to play and nothing to type into.
A blank row is left out, because the reading view shows only what reads.

## `private void PDisplayTranscriptionShow(LEntryDraft draft)`

Rebuilds the rows from the draft, in the order the entry keeps them.
The row in the glyph scheme is left out, because the glyph section beneath shows it as characters.
