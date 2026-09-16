# PReflex.cs

## `public partial class PEditor`

The reflex rows of the input panel, under the headword at the head of the reading stack.
The rows are the same rows the reading view prints, with bare fields where it prints text.
The remark has a field of its own after the note.
The region is only a hover on the language field.
The folded languages hide under the same fold the view has, and the fold state is shared with it.
Every change is a request to the engine, and the rows are rebuilt from the draft it answers with.

## `private static string PReflexRequestFormat(long id, string field)`

The key a deferred request of one row and one field is held under.

## `internal void PReflexAddHandle(object sender, ExecutedRoutedEventArgs e)`

Adds a row after the one the plus was pressed on, in its language and kind.
Without a row it adds a blank one at the end.

## `internal void PReflexRemoveHandle(object sender, ExecutedRoutedEventArgs e)`

Drops the row the minus was pressed on.

## `internal void PReflexMainHandle(object sender, ExecutedRoutedEventArgs e)`

Flips the main mark of the row the star was pressed on.

## `internal void PReflexRebuildHandle(object sender, ExecutedRoutedEventArgs e)`

Asks the engine to drop the entry's reflex rows and fetch them again.
The button sits at the top right corner of the block.
The table keeps its width and height for the fetch, so the button stays where it was pressed.
A draft not yet stored as an entry has nothing to fetch for, so the press does nothing.
A failed ask is ignored, and the rows stay as they are.

## `private void PReflexChangeHandle(object? sender, PropertyChangedEventArgs e)`

A text change becomes a respelling request while the row prints its respelling and a reading request otherwise.

Defers a request for the field that changed, so typing is sent in one piece.
A language change also remarks the leads at once, so the language prints on the right row while typing.

## `private void PReflexShow(LEntryDraft draft)`

Rebuilds the rows from the draft, keeping a row that is still being typed into.
The stack is shown when the language declares a rule or the draft carries a row, and hidden otherwise.
The folded languages are read from the rules of the draft's language, and the fold is applied last.
The anchor labels are written from the fanqie rows the editor last read.

## `internal void PReflexAnchorShow()`

Writes every row's anchor label from the fanqie rows held, under the headword as it now reads.

## `internal void PReflexPendingShow()`

Shows the fetching line and turns the fetch-again icon while the engine fills the entry.
Both settle when the fill answers, which reaches here by the reflex bulletin.
The held table size is let go once the fill is over, so the new rows size the table again.
An unsaved entry has no fill and shows nothing.

## `internal void PReflexFoldHandle(object sender, RoutedEventArgs e)`

Opens or closes the fold as the toggle under the rows was pressed.

## `private void PReflexPrepare(LEntryDraft draft)`

Asks the engine to fill a stored entry that has no rows yet.
The fill reaches the draft through the engine and lands here by bulletin.
The fetching line is shown at once, since the ask starts the fill before it returns.
An unsaved entry has no id and is not asked.

## `private PReflexItem PReflexCreate(LReflexDraft reflex)`

A row for a draft row, listened to for changes.
It prints the form the switch picks for the row's own language.

## `private PReflexItem PReflexUpdate(PReflexItem row, LReflexDraft reflex)`

Brings a row up to the draft, leaving a field alone while its own request is still deferred.
A row whose respelled, phonemic or folded state no longer matches its language is rebuilt.
Those states are fixed when the row is created.
The anchors are taken from the draft as they are, since the engine holds them and the tick sent them.
The tone is taken from the draft's anatomy the same way, since the engine cuts it.

## `private void PReflexClear()`

Drops every row and the fanqie rows held.
The stack, the fetching line and the turning icon hide with them.
