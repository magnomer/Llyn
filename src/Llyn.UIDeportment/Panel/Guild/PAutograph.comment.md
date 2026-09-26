# PAutograph.cs

## `public class PAutograph : UserControl`

The Author edit area as a control of its own, held by the authors panel.
It holds no state, since the guild deportment holds the autograph desk and the union verdict.
Every handler forwards what the control carries, and every update writes what the deportment answers.

## `public PAutograph()`

Loads the markup from the Veneer, wears it as content, and copies its name scope.
Subscribes the name and union fields and the union list's clicks, and attaches the union row fill.

## `private TextBox PAutographName`

Each named part of the markup is read through `FindName`, so call sites keep the old generated names.

## `internal void PAutographAttach(LGuild guild)`

Takes the deportment the panel built and listens to its autograph desk.
The start notice, the desk's own bulletins and the draft notice are attached in the panel's old order.

## `internal void PAutographTallyShow(LVita vita)`

Writes the two count chips from the vita sheet the panel read, so both sides show the same counts.

## `internal void PAutographModeUpdate()`

Shows the union section or its unsaved notice by the deportment's union verdict.
The panel calls it whenever it writes its own mode.

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

## `private void PAutographUnionSelect(object sender, RoutedEventArgs e)`

A click on a union row folds the held Author into that one.
