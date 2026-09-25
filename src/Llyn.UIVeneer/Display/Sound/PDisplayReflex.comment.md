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

## `private void PDisplayReflexShow(LEntryDraft draft)`

Rebuilds the rows from the draft, in the order the entry keeps them, and marks the lead of each language.
Each row prints the form the switch picks for that row's own language, asked of the engine per row.
The folded languages are read from the rules of the entry's language, and the fold is applied last.
The anchor labels are left to the caller, since the entry show writes them with the fanqie box.

## `private void PReflexPendingShow(long id)`

Shows the fetching line while the engine fills the entry, and hides it otherwise.
A failed ask counts as no fill.

## `private void PDisplayReflexLoad(long id)`

Re-reads the shown entry after a fill and rebuilds the lines alone.
The rest of the view stays as it stands.
A failed read leaves the lines as they were.
The fill brings no fanqie show, so the anchor labels are written here from rows read afresh.
The headword is the one on screen, so the engine's answer is not handed back to it.
The fetching line settles with the same bulletin, since the fill is over when it is raised.

## `private void PReflexFoldHandle(object sender, RoutedEventArgs e)`

Opens or closes the fold as the toggle under the stack was pressed.
