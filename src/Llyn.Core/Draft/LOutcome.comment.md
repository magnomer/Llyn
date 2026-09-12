# LOutcome.cs

## `public sealed record LOutcome(`

What one draft commit produced.
It is plain data and knows nothing of files or the database.
The map is an output of the commit, never an input to anything.

**Parameters**

- `LOutcomeEntry` — The entry as stored after the commit.
- `LOutcomeIdentity` — One pair per negative id the draft held, from that id to the row id the database gave.
  Pairs sit in the order the engine created rows.
