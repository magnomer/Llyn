# PGuild.xaml.cs

## `public partial class PGuild : UserControl`

The authors panel: the workspace browsed by the people its Sources credit.
It is the sources panel's shape read through a different question, so it holds the same three columns.
It owns a reader and an editor over one Author, and answers the engine rather than its own visibility.
The roll, the oeuvre, the vita and the autograph each live in a partial file of their own.

## `internal void PGuildAttach(PWindow host, LEngine engine)`

Binds the panel to the window and the engine, wires every list to its rows, and starts listening for bulletins.

## `internal void PGuildReset()`

Clears the panel and reads the roll again, for when the workspace itself changed.

## `internal bool PGuildChangeCheck()`

Whether the edit area holds a name not yet saved.

## `internal bool PGuildDraftFinish(bool store)`

Finishes the held edit before the panel is left, saving it or dropping it as asked.
A failed save keeps the panel where it is, so the user can read why.

## `internal void PGuildClose()`

Closes the dropdowns, so nothing stays open over a window that is going.

## `internal bool PGuildLeaveConfirm()`

Asks the window whether an unsaved name may be left, in the words every panel uses.

## `private void PGuildPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Print is live only while a Source stands in the colophon, because an Author has no page to print.

## `private async void PGuildPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the Source read in the colophon, through the window's press run.

## `private void PGuildScribeHandle(object sender, RoutedEventArgs e)`

The mode toggle, between reading and editing the chosen Author.
Leaving the edit side asks about an unsaved name first, and stays on it when the user says no.
Turning to the edit side with no Author chosen clears the panel, because there is nothing to edit.

## `internal void PGuildScribeShow(bool editing)`

Swaps the reading and the edit area, ticks the toggle to match, and remembers the side for the next session.

## `internal void PGuildScribeRestore(bool editing)`

Reopens the side the last session ended on, but only the reading side while no Author is chosen.

## `internal void PGuildClear()`

Returns the panel to nothing chosen: both vistas stand on nothing, every list read afresh, the reading side shown.
