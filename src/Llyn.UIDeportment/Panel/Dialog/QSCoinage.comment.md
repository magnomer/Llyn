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
The answer comes back as typed, or `null` when the user retreated.
The clerk behind the create gate trims it, since trimming is a data rule.

## `private void QSCoinageWordingRefine(object sender, TextChangedEventArgs e)`

The mint button follows the field, lit only while `CSCoinage` accepts the wording.

## `private void QSCoinageMintObserve(object sender, RoutedEventArgs e)`

Closes the dialog with an answer.
The button is dark until `CSCoinage` accepts the wording, so a click or the enter key never reaches it blank.
