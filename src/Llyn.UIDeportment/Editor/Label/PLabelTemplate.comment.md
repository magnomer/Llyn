# PLabelTemplate.cs

## `public class PLabelTemplate : ResourceDictionary`

This dictionary shares label chips and editing controls across cards without owning their state.

The host's fill subscribes each forwarder on the realized part, where an event attribute stood before.

## `private readonly PEditor _pLabelHost`

The editor tracks which card owns a label interaction in the shared dictionary.

## `internal PLabelTemplate(PEditor host)`

The host routes shared label controls into the current editing context.
Merges the markup the Veneer holds, since the dictionary carries no class of its own there.

## `internal void PLabelChipHandle(object sender, RoutedEventArgs e)`

Chip activation delegates label selection to the editor for the active card.

## `internal void PLabelCaretHandle(object sender, KeyEventArgs e)`

The editor interprets caret keys because label editing depends on card state.

## `internal void PLabelCloseHandle(object sender, RoutedEventArgs e)`

The editor removes the label so its draft remains consistent with the card.

## `internal void PLabelFocusHandle(object sender, MouseButtonEventArgs e)`

Focus handling lets the editor place the caret in the correct card's label field.
