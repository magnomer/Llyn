# TTrove.cs

## `public sealed class TTrove`

Covers the per-session trove holding the candidates a lookup found.
Several candidates sharing one order survive a save and a read unchanged.
A hold is read back only for the same word and language.
A find with no phonetic at all is not held.
A recording hold keeps every variety a source gave, read back only for the same word and language.
A recording find with no address at all is not held.
A transcription hold is kept once per scheme, so two schemes of one draft read back separately.
Clearing a session drops every scheme it held and leaves another session's holds standing.
