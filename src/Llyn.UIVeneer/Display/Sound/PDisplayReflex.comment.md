# PDisplayReflex.cs

## `public partial class PDisplay`

The reflex lines of a shown entry, under its headword at the head of the reading stack.
Each language leads its rows, and each row prints its kind, its reading and its note, the main one accented.
Hovering a language name shows the region its readings are taken from.
A reading's remark, such as literary, prints after its note.
The languages the pack folds away stand below the rest, hidden until the fold under the stack is opened.
The fold state is shared with the editor and kept for the session.
So it stays as last set from entry to entry.
There is nothing to play and nothing to type into.
A blank row is left out, because the reading view shows only what reads.

## `private void PDisplayReflexStart(long id)`

Asks the engine to fill the entry when it has no rows, once per entry show.
The engine declines when rows stand, a fill runs, or the sources missed it this session.
A failed ask is ignored, and the lines then show what is stored.

## `private void PDisplayReflexShow(long id, LEntryDraft draft)`

Rebuilds the rows from the draft, in the order the entry keeps them, and marks the lead of each language.
Each row prints the form the switch picks for that row's own language, asked of the engine per row.
The folded languages are read from the rules of the entry's language, and the fold is applied last.
The anchor labels are written from the fanqie rows of `id` read afresh, and again when the rows land.

## `private void PReflexPendingShow(long id)`

Shows the fetching line while the engine fills the entry, and hides it otherwise.
A failed ask counts as no fill.

## `private void PDisplayReflexLoad(long id)`

Re-reads the shown entry after a fill and rebuilds the lines alone.
The rest of the view stays as it stands.
A failed read leaves the lines as they were.
The fetching line settles with the same bulletin, since the fill is over when it is raised.

## `private void PReflexFoldHandle(object sender, RoutedEventArgs e)`

Opens or closes the fold as the toggle under the stack was pressed.

## `internal static PReflexItem PReflexItemCreate(`

A row for one draft reflex, asking the engine for its language's respelling state and phonemic flag.
The row is folded when its language is in `folded`.
Shared with the editor, whose rows are built by the same rule.

## `internal static HashSet<string> PReflexFoldRead(LWindow window, string language)`

The languages the pack of `language` folds away, read from its reflex rules.
Empty for a blank language or a pack without rules.

## `internal static void PReflexLeadApply(IReadOnlyList<PReflexItem> rows)`

Marks each row that opens a run of one language as its lead, so the language prints once per run.
Shared with the editor, whose rows lead by the same rule.

## `internal static void PReflexAnchorApply(`

Writes each row's anchor label from `fanqie`, and whether the row may be anchored at all.
The engine decides both, so a row it refuses prints nothing.
Shared by both panes, so the label reads the same in the editor and the reading view.

## `internal static void PReflexFoldToggle(IReadOnlyList<PReflexItem> rows, ToggleButton fold)`

Takes the fold state from the toggle and applies it to the rows.

## `internal static void PReflexFoldApply(IReadOnlyList<PReflexItem> rows, ToggleButton fold)`

Hides every folded row while the fold is closed and shows it while open.
The toggle shows the state and is visible only when some row is folded.
