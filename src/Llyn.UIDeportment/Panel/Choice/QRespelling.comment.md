# QRespelling.cs

## `internal sealed class QRespelling`

The respelling switch the user ticks in the settings panel.
It is kept as a setting so the next run opens with it.
The engine raises a settings bulletin when it changes, and every open reading redraws in the form now picked.
Nothing is refetched and nothing stored changes, because both forms are already kept.
The same bulletin redraws this panel.

## `internal QRespelling(FrameworkElement settings)`

Wires the switch's click once, through its markup name, as the settings panel is built.

## `internal void QRespellingIntroduce(CLedger ledger)`

Puts the switch to work on the ledger the panel hands over at introduction.

## `internal void QRespellingRefine(bool chosen)`

Paints the switch from the stored setting the panel hands over.
Setting `IsChecked` in code raises no `Click`, so a painted switch writes nothing back.

## `private void QRespellingObserve(object sender, RoutedEventArgs e)`

Only hands the raw switch to one ledger save.
