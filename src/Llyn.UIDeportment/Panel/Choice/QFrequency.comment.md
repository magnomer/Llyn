# QFrequency.cs
Hash: `0b63620f68a1644a`

## `internal sealed class QFrequency`

The frequency switch the user ticks in the settings panel.
It is kept as a setting so the next run opens with it.
Nothing already stored is fetched or dropped when it changes.
The next save or first display of an unfilled entry reads the switch, because the engine checks it per fill.
The settings bulletin the engine raises redraws the panel.

## `internal QFrequency(FrameworkElement settings)`

Wires the switch's click once, through its markup name, as the settings panel is built.

## `internal void QFrequencyIntroduce(CLedger ledger, CEnvoy envoy)`

Puts the switch to work on the ledger the panel hands over at introduction.
It keeps the window's envoy, which the ledger shows a failed save through.

## `internal void QFrequencyRefine(bool chosen)`

Paints the switch from the stored setting the panel hands over.
Setting `IsChecked` in code raises no `Click`, so a painted switch writes nothing back.

## `private void QFrequencyObserve(object sender, RoutedEventArgs e)`

Only hands the raw switch to one ledger save, with the kept envoy.
A failed save raises the ledger again, which paints the switch back.
