# PLabelTemplate.xaml.cs

## `public partial class PLabelTemplate : ResourceDictionary`

This dictionary shares label chips and editing controls across cards without owning their state.

## `private readonly PEditor _pLabelHost`

The editor tracks which card owns a label interaction in the shared dictionary.

## `internal PLabelTemplate(PEditor host)`

The host routes shared label controls into the current editing context.

## `private void PLabelChipHandle(object sender, RoutedEventArgs e)`

Chip activation delegates label selection to the editor for the active card.

## `private void PLabelCaretHandle(object sender, KeyEventArgs e)`

The editor interprets caret keys because label editing depends on card state.

## `private void PLabelCloseHandle(object sender, RoutedEventArgs e)`

The editor removes the label so its draft remains consistent with the card.

## `private void PLabelFocusHandle(object sender, MouseButtonEventArgs e)`

Focus handling lets the editor place the caret in the correct card's label field.
