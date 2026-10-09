# QInflection.cs
Hash: `ee61dd1e41f56e82`

## `internal sealed class QInflection`

The Inflection page of the settings panel, which holds the custom analysis switch the user ticks.
The switch is kept in the workspace settings, so the next run opens with it.
Turning it on makes the inflection box use the pack's custom sheets and mark letters that differ from the rules.
A saved change raises the settings bulletin, which redraws the panel.

## `internal QInflection(FrameworkElement settings)`

Wires the switch's click once, through its markup name, as the settings panel is built.

## `internal void QInflectionIntroduce(CLedger ledger, CEnvoy envoy)`

Puts the switch to work on the ledger the panel hands over at introduction.
It keeps the window's envoy, which the ledger shows a failed save through.

## `internal void QInflectionRefine()`

Paints the switch from the setting the ledger reads.
Setting `IsChecked` in code raises no `Click`, so a painted switch writes nothing back.

## `private void QInflectionObserve(object sender, RoutedEventArgs e)`

Only hands the raw switch to one ledger save, with the kept envoy.
A failed save raises the ledger again, which paints the switch back.
