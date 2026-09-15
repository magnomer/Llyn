# PReflex.cs

## `public partial class PEditor`

The reflex rows of the input panel, under the headword at the head of the reading stack.
The rows are the same rows the reading view prints, with bare fields where it prints text.
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
A draft not yet stored as an entry has nothing to fetch for, so the press does nothing.
A failed ask is ignored, and the rows stay as they are.

## `private void PReflexChangeHandle(object? sender, PropertyChangedEventArgs e)`

A text change becomes a respelling request while the row prints its respelling and a reading request otherwise.

Defers a request for the field that changed, so typing is sent in one piece.
A language change also remarks the leads at once, so the language prints on the right row while typing.

## `private void PReflexShow(LEntryDraft draft)`

Rebuilds the rows from the draft, keeping a row that is still being typed into.
The stack is shown when the language declares a rule or the draft carries a row, and hidden otherwise.

## `private void PReflexPrepare(LEntryDraft draft)`

Asks the engine to fill a stored entry that has no rows yet.
The fill reaches the draft through the engine and lands here by bulletin.
An unsaved entry has no id and is not asked.

## `private PReflexItem PReflexCreate(LReflexDraft reflex)`

A row for a draft row, listened to for changes.
It prints the form the switch picks for the row's own language.

## `private PReflexItem PReflexUpdate(PReflexItem row, LReflexDraft reflex)`

Brings a row up to the draft, leaving a field alone while its own request is still deferred.
A row whose respelled state no longer matches the switch and its language is rebuilt.
That state is fixed when the row is created.

## `private void PReflexClear()`

Drops every row and hides the stack.
