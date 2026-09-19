# PSender.cs

## `internal static class PSender`

Reads which templated item, tag or command parameter a routed event's sender carries.
A veneer handler may not pattern-match, so the `is` unwrap lives here and answers a `P` value or null.
It owns no state.

## `internal static PSenderItem? PSenderItemRead<PSenderItem>(object sender)`

The item the sender's data context holds when it is one of the asked type, else null.

## `internal static string? PSenderTagRead(object sender)`

The string the sender's `Tag` holds, else null.

## `internal static PSenderItem? PSenderParameterRead<PSenderItem>(ExecutedRoutedEventArgs e)`

The command parameter when it is one of the asked type, else null.
