# QLocalization.cs
Hash: `10e7803b573f882f`

## `internal sealed class QLocalization`

The interface language the user picks in the settings panel.
It is kept as a setting so the next run opens in it.
The language alone is handed downstream, never the whole settings record, so a geometry saved elsewhere is not overwritten.

## `internal QLocalization(FrameworkElement settings)`

Names the language box's value path and wires its selection change once.
The box starts empty, since its items wait for the first ledger state.

## `internal void QLocalizationIntroduce(CLedger ledger)`

Puts the box to work on the ledger the panel hands over at introduction.

## `internal void QLocalizationRefine(IReadOnlyList<KeyValuePair<string, string>> languages, string localization)`

Paints the box from one ledger state the panel hands over.
The items are built once, from the first state.
Setting the selection raises the selection change, and the observer writes the value back.
That write leaves the record equal, so the engine neither saves nor raises a bulletin.

## `private void QLocalizationChoiceRefine(IReadOnlyList<KeyValuePair<string, string>> languages)`

Fills the box with one item per interface language the ledger lists.
Each item shows the native name and carries its language code as `Tag`.
The markup lists no language, so a new catalog file needs no edit to either side.

## `private void QLocalizationObserve(object sender, SelectionChangedEventArgs e)`

Only hands the raw language to one ledger save.
The ledger state the save brings back applies the catalog.

## Inline notes

### `if (sender is not ComboBox { SelectedValue: string language })`

The language is read off the box the event came from, the raw value the observer heard.
A cleared box has nothing to save.
