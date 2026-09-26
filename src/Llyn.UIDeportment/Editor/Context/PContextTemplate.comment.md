# PContextTemplate.cs

## `public class PContextTemplate : ResourceDictionary`

This dictionary keeps context chips visually separate while the editor owns their state and keyboard behavior.

The host's fill subscribes each forwarder on the realized part, where an event attribute stood before.

## `private readonly PEditor _pContextHost`

The owning editor resolves each context event against the card currently being edited.

## `internal PContextTemplate(PEditor host)`

The host gives shared templates a route back to the editor that owns context interactions.
Merges the markup the Veneer holds, since the dictionary carries no class of its own there.

## `internal void PContextChipHandle(object sender, RoutedEventArgs e)`

Chip activation asks the editor to apply its context action to the active card.

## `internal void PContextCaretHandle(object sender, KeyEventArgs e)`

Caret keys need editor context to preserve navigation across editable context chips.

## `internal void PContextCloseHandle(object sender, RoutedEventArgs e)`

Closing a context chip delegates removal to the editor that tracks card state.

## `internal void PContextFocusHandle(object sender, MouseButtonEventArgs e)`

Field focus delegates to the editor so it can choose the correct card caret.
