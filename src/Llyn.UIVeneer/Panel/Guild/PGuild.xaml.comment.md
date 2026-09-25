# PGuild.xaml.cs

## `public partial class PGuild : UserControl`

The authors panel: the workspace browsed by the people its Sources credit.
It is the sources panel's shape read through a different question, so it holds the same three columns.
Every decision lives in [LGuild](../../Llyn.UIDeportment/LGuild.comment.md), and this file writes controls on notice.
The roll, the oeuvre, the vita, the autograph and the colophon are all served from one file.

## `internal void PGuildAttach(PWindow host)`

News the deportment with the window's dialogs as seams, subscribes its notices, and wires the two lists.
The print command binding is added last, so no can-execute query ever meets a deportment not yet built.

## `internal void PGuildVistaRestore()`

The deportment starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Hands the vistas down, attaches the bulletins the panel follows, restores the order and kind menus, and lists.

## `internal bool PGuildChangeCheck()`

Whether the edit area holds a name not yet saved.

## `internal bool PGuildDraftFinish(bool store)`

Finishes the held edit before the panel is left, saving it or dropping it as asked.

## `internal bool PGuildLeaveConfirm()`

The panel's question before its unsaved work goes out of sight, asked by the window.

## `internal void PGuildScribeRestore(bool editing)`

Reopens the side the last session ended on.

## `internal void PGuildClose()`

Closes the dropdowns, so nothing stays open over a window that is going.

## `private bool PGuildDiscardConfirm()`

The leave seam: the window's discard question over this panel's finish.

## `private bool PGuildRemovalConfirm(int works)`

The removal seam: the window's delete question worded by how many Sources credit the Author.

## `private void PRollUpdate()`

Lists the roll afresh and reads the vita and the count chips, which follow the same Author.

## `private void PVitaUpdate()`

Writes the vita from its read sheet: name, counts, fellows and citing places, and shows or hides the body.
The autograph's count chips are written from the same sheet.

## `private void POeuvreUpdate()`

Lists the oeuvre afresh with its empty text and refreshes the colophon's tally.

## `private void PGuildSourceUpdate(LDraft draft)`

A Source was loaded for the colophon, so its sheet is composed and shown.

## `private void PGuildModeUpdate()`

Writes every visibility and enablement off the deportment's verdicts.

## `private void PGuildObserverAttach()`

Hands the autograph desk to the registration below, from a method that only reads the deportment.

## `private void PAutographObserverAttach(LDesk desk)`

Registers the desk's own draft and state updates on it, marshalled to the window's thread.

## `private void PAutographStartUpdate()`

A tenure was started: the union field is emptied and the name takes focus.
The desk's bulletins were registered on the desk at attach time, so nothing is attached here.

## `private void PAutographDraftUpdate(LDraft draft)`

The held draft was read again, so the name field shows its name.

## `private void PAutographNameHandle(object sender, TextChangedEventArgs e)`

Every keystroke in the name is deferred to the desk as a raw name request.

## `private void PAutographUnionHandle(object sender, TextChangedEventArgs e)`

Lists the Authors the typed name matches, for the user to fold this one into.

## `private void PVitaCitationHandle(object sender, RoutedEventArgs e)`

A citing place leads to its Example or its Entry, as the row knows.

## `private async void PGuildPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the Source read in the colophon, through the window's press run.

## `internal void PGuildVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the panel only shows what it is told.

## `private void PGuildRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void PGuildAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.

## `private void PGuildUndoHandle(object sender, RoutedEventArgs e)`

Walks the autograph desk back one step, with the caret kept where it was.

## `private void PGuildRedoHandle(object sender, RoutedEventArgs e)`

Walks the autograph desk forward one step, the mirror of the undo.

## `private void PGuildChronicleUpdate()`

Lights the two chronicle buttons only while the desk has a step to walk.
It runs whenever the desk reports its state again.

## `internal long PGuildVoyageRead()`

The Author the panel shows, read off the guild panel as the station of this panel.

## `internal void PRollAuthorShow(long id)`

Shows one Author by id, for a jump the window makes from another panel.
It asks nothing, because the window asks before it jumps.
