# PContextTemplate.xaml.cs

## `public partial class PContextTemplate : ResourceDictionary`

This dictionary keeps context chips visually separate while the editor owns their state and keyboard behavior.

## `private readonly PEditor _pContextHost`

The owning editor resolves each context event against the card currently being edited.

## `internal PContextTemplate(PEditor host)`

The host gives shared templates a route back to the editor that owns context interactions.

## `private void PContextChipHandle(object sender, RoutedEventArgs e)`

Chip activation asks the editor to apply its context action to the active card.

## `private void PContextCaretHandle(object sender, KeyEventArgs e)`

Caret keys need editor context to preserve navigation across editable context chips.

## `private void PContextCloseHandle(object sender, RoutedEventArgs e)`

Closing a context chip delegates removal to the editor that tracks card state.

## `private void PContextFocusHandle(object sender, MouseButtonEventArgs e)`

Field focus delegates to the editor so it can choose the correct card caret.
