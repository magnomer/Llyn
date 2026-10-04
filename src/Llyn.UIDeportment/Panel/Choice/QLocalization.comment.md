# QLocalization.cs
Hash: `099a4d0f00dfe588`

## `internal sealed class QLocalization`

The interface language the user picks in the settings panel.
It is kept as a setting so the next run opens in it.
The language alone is handed downstream, never the whole settings record, so a geometry saved elsewhere is not overwritten.

## `internal QLocalization(FrameworkElement settings)`

Names the language box's value path and wires its selection change.
The box starts empty, since its items wait for the first ledger state.

## `internal void QLocalizationIntroduce(CLedger ledger, CEnvoy envoy)`

Puts the box to work on the ledger the panel hands over at introduction.
It keeps the window's envoy, which the ledger shows a failed save through.

## `internal void QLocalizationRefine(IReadOnlyList<KeyValuePair<string, string>> languages, string localization)`

Paints the box from one ledger state the panel hands over.
The items are rebuilt whenever the ledger's list differs from what the box shows.
The observer is unhooked while the items and the selection are painted, and hooked again after.
So a selection the driver set itself raises no save.

## `private void QLocalizationChoiceRefine(IReadOnlyList<KeyValuePair<string, string>> languages)`

Fills the box with one item per interface language the ledger lists.
It keeps the items when each one already matches the list by name, code and order.
Each item shows the native name and carries its language code as `Tag`.
The markup lists no language, so a new catalog file needs no edit to either side.

## `private void QLocalizationObserve(object sender, SelectionChangedEventArgs e)`

Only hands the raw language a user picked to one ledger save, with the kept envoy.
A failed save raises the ledger again, which paints the box back.
The ledger state the save brings back applies the catalog.

## Inline notes

### `if (sender is not ComboBox { SelectedValue: string language })`

The language is read off the box the event came from, the raw value the observer heard.
A cleared box has nothing to save.
