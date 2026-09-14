# PSCoinage.xaml.cs

## `public partial class PSCoinage : Window`

The wording the dialog was closed with, or nothing when it was dismissed.
Only the mint button sets an answer, so any other way out makes no row.

## `internal static string? PSCoinageShow(Window owner, string message)`

Shows the dialog over its owner, saying `message`, and waits for the wording.
The field takes focus on open, so the user types straight away.
The answer comes back trimmed, or `null` when the user retreated.

## `private void PSCoinageWordingHandle(object sender, TextChangedEventArgs e)`

The mint button follows the field, lit only while it holds more than blanks.

## `private void PSCoinageMintHandle(object sender, RoutedEventArgs e)`

Takes the trimmed wording as the answer and closes.
An empty field is a guard against the enter key, since the button is dark then anyway.
