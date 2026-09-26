# QSCoinage.cs

## `internal sealed class QSCoinage`

The wording the dialog was closed with, or nothing when it was dismissed.
Only the mint button closes with a result, so any other way out makes no row.
The window is the veneer's `PSCoinage` page, pulled fresh by contract ID and held in `_qsCoinageSurface`.

## `private QSCoinage(Window owner, string message)`

Pulls the window, sets its owner and subscribes the field and the mint button.

## `private TextBlock QSCoinageMessage`

Each named part is pulled from the window by its contract ID.

## `internal static string? QSCoinageShow(Window owner, string message)`

Shows the dialog over its owner, saying `message`, and waits for the wording.
The field takes focus on open, so the user types straight away.
The answer comes back trimmed, or `null` when the user retreated.

## `private void QSCoinageWordingHandle(object sender, TextChangedEventArgs e)`

The mint button follows the field, lit only while `CSCoinage` accepts the wording.

## `private void QSCoinageMintHandle(object sender, RoutedEventArgs e)`

Closes with an answer only when `CSCoinage` accepts the wording.
Otherwise the result stays unset and the dialog stays open.
The check guards against the enter key, since the button is dark then anyway.
