# TTenorEntry.cs
Hash: `b070e44ef3675130`

## `public sealed class TTenorEntry`

Covers how the tenor panel starts, closes and opens an entry on a real workspace.
It builds its panel through `TTenor.TTenorPrepare`.
New asks the wording under the Register key.
A padded wording opens the trimmed Register as an arrival does, chosen with no usage.
A blank wording shows the refusal through the envoy and opens nothing, and a retreat makes nothing.
A changed fresh entry that stays gets only the leave question.
One that is discarded goes on to the wording.
New starts an entry without asking the wording once a Register is chosen.
A fresh entry under a chosen Register carries it from its first paint and lists under it once stored.
A fresh entry with an entry shown and no Register chosen is blank and unchanged.
Closing the panel drops the fresh draft the editor held.
A cohort row select opens the entry with no station, and a stayed leave question opens nothing.

## `private static LEntry TTenorEntrySave(LEngine engine)`

Saves the one hearth entry the row and fresh tests need.

## `private static void TTenorChangePrepare(LEngine engine, CTenor tenor)`

Leaves a changed fresh entry held with no Register chosen, so New asks the leave question.
