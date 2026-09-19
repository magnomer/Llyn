# PSender.cs

## `internal static class PSender`

Reads which templated item, tag or command parameter a routed event's sender carries.
A veneer handler may not pattern-match, so the `is` unwrap lives here and answers a `P` value or null.
It owns no state.

## `internal static PSenderItem? PSenderItemRead<PSenderItem>(object sender)`

The item the sender's data context holds when it is one of the asked type, else null.

## `internal static PSenderItem? PSenderSourceRead<PSenderItem>(RoutedEventArgs e)`

The item the event's original source carries, for a click bubbled up to the list rather than the row.

## `internal static string? PSenderTagRead(object sender)`

The string the sender's `Tag` holds, else null.

## `internal static PSenderItem? PSenderParameterRead<PSenderItem>(ExecutedRoutedEventArgs e)`

The command parameter when it is one of the asked type, else null.

## `internal static string? PSenderTextRead(ExecutedRoutedEventArgs e)`

The command parameter as text, or null when it is none.

## `internal static bool? PSenderFlagRead(ExecutedRoutedEventArgs e)`

The command parameter as a flag, or null when it is none.

## `internal static PSenderItem? PSenderFocusRead<PSenderItem>()`

The item the focused element's data context holds, for a popup click that acts on the field behind it.
The click tunnels through before the focus moves, so the field being typed into is still the focused one.

## `internal static string PSenderKeyRead(KeyEventArgs e)`

The name of a key a deportment answers to, or empty for any other key.
The deportment may not name the framework's key type, so the four it acts on cross as words.
