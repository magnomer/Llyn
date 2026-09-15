# PDisplayReflex.cs

## `public partial class PDisplay`

The reflex lines of a shown entry, under its headword at the head of the reading stack.
Each language leads its rows, and each row prints its kind, its reading and its note, the main one accented.
There is nothing to play and nothing to type into.
A blank row is left out, because the reading view shows only what reads.

## `private void PDisplayReflexStart(long id)`

Asks the engine to fill the entry when it has no rows, once per entry show.
The engine declines when rows stand, a fill runs, or the sources missed it this session.
A failed ask is ignored, and the lines then show what is stored.

## `private void PDisplayReflexShow(LEntryDraft draft)`

Rebuilds the rows from the draft, in the order the entry keeps them, and marks the lead of each language.
Each row prints the form the switch picks for that row's own language, asked of the engine per row.

## `private void PDisplayReflexLoad(long id)`

Re-reads the shown entry after a fill and rebuilds the lines alone.
The rest of the view stays as it stands.
A failed read leaves the lines as they were.

## `internal static void PReflexLeadApply(IReadOnlyList<PReflexItem> rows)`

Marks each row that opens a run of one language as its lead, so the language prints once per run.
Shared with the editor, whose rows lead by the same rule.
