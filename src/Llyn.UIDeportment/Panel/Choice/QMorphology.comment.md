# QMorphology.cs

## `internal sealed class QMorphology`

The morphology switch the user ticks in the settings panel.
It is kept as a setting so the next run opens with it.
Turning it off cancels any fetch still in flight, and the engine does that under its own gate.
Forms already stored stay, because the switch governs asking, not keeping.
The next display of an unfilled entry reads the switch, because the engine checks it per fetch.
The settings bulletin the engine raises redraws the panel.

## `internal QMorphology(FrameworkElement settings)`

Wires the switch's click once, through its markup name, as the settings panel is built.

## `internal void QMorphologyIntroduce(CLedger ledger)`

Puts the switch to work on the ledger the panel hands over at introduction.

## `internal void QMorphologyRefine(bool chosen)`

Paints the switch from the stored setting the panel hands over.
Setting `IsChecked` in code raises no `Click`, so a painted switch writes nothing back.

## `private void QMorphologyObserve(object sender, RoutedEventArgs e)`

Only hands the raw switch to one ledger save.
