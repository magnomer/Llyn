# QAutograph.cs

## `internal sealed class QAutograph`

The driver of the Author edit area, a veneer page the guild page places.
It holds no state, since the guild controller holds the autograph desk and the union verdict.
Every handler forwards what the page carries, and every update writes what the controller answers.

## `internal QAutograph(UserControl surface)`

Takes the edit page as its surface.
Subscribes the name and union fields and the union list's clicks, and attaches the union row fill.

## `private TextBox QAutographName`

Each part of the page is pulled by its contract ID through `QContract.QContractFind`.

## `internal void QAutographAttach(LGuild guild)`

Takes the controller the guild driver built and listens to its autograph desk.
The start notice, the desk's own bulletins and the draft notice are attached in the old order.

## `internal void QAutographTallyShow(CVita vita)`

Writes the two count chips from the vita sheet the guild read, so both sides show the same counts.

## `internal void QAutographModeUpdate()`

Shows the union section or its unsaved notice by the controller's union verdict.
The guild driver calls it whenever it writes its own mode.

## `private void QAutographObserverAttach(LDesk desk)`

Registers the desk's own draft and state updates on it, marshalled to the window's thread.

## `private void QAutographStartUpdate()`

A tenure was started: the union field is emptied and the name takes focus.
The desk's bulletins were registered on the desk at attach time, so nothing is attached here.

## `private void QAutographDraftUpdate(CDraft draft)`

The held draft was read again, so the name field shows its name.

## `private void QAutographNameHandle(object sender, TextChangedEventArgs e)`

Every keystroke hands the name to the desk's `QQuill`, which defers it as the old raw request did.

## `private void QAutographUnionHandle(object sender, TextChangedEventArgs e)`

Lists the Authors the typed name matches, for the user to fold this one into.

## `private void QAutographUnionSelect(object sender, RoutedEventArgs e)`

A click on a union row folds the held Author into that one.
