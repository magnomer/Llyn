# PLinkTemplate.xaml.cs

## `public partial class PLinkTemplate : ResourceDictionary`

This dictionary presents link chips and their editor while the owning editor manages link state.

## `private readonly PEditor _pLinkHost`

The host identifies the active card when shared link controls raise events.

## `internal PLinkTemplate(PEditor host)`

The editor reference preserves card ownership across shared link templates.

## `private void PLinkDropHandle(object sender, RoutedEventArgs e)`

Chip activation asks the editor to follow the selected link action for its card.

## `private void PLinkCaretHandle(object sender, KeyEventArgs e)`

Caret navigation is delegated because the editor knows the active link draft.

## `private void PLinkCloseHandle(object sender, RoutedEventArgs e)`

The editor removes the link from the correct card and its draft.

## `private void PLinkFocusHandle(object sender, MouseButtonEventArgs e)`

The editor places focus in the active card's link field.
