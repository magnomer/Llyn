# POccurrenceItem.cs

## `internal sealed class POccurrenceItem`

One Entry as a row of the repertoire panel's entry list.
It mirrors the taxonomy panel's row, because every middle column answers the same shape of question.
That question is which Entries reference the Situation chosen in the catalog beside them.
An Entry referencing from several of its cards is still one row, because the row stands for the Entry.
The headword and the shown name are separate, so two Entries sharing a headword can be numbered apart.
The mark saying which row the reader stands on is the one thing that changes after the row is built.
