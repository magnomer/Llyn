# PEtymologyField.cs

## `public partial class PEditor`

The editor's half of the etymology field: the source links, the narrative and its spans.
The field itself draws both sides, and every change goes through one card gate.
Resolution goes through the prospect menu, as a translation does, and the engine trims the typed word.

## `internal void PEtymologyAttach()`

Binds the field's commands and listens to the narrative box once, when the editor is built.

## Inline notes

### `internal void PEtymologyRefine(CEntryDraft draft)`

Hands the field the source links the engine resolved, since the draft holds ids alone.
A link whose target could not be read is left out rather than drawn blank.

### `private void PEtymologyWriteObserve(object sender, TextChangedEventArgs e)`

The narrative is deferred like every other typed field, so one keystroke is not one request.

### `private void PEtymologyAddRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the menu of the entries the typed word matches, from the editor's mention read.
Only the entry's Return key raises the command, so its parameter and source are the entry and its box.
The pick is heard by `PProspectPickObserve`, which finds the entry from the box the menu stands at.

### `private void PEtymologyRemoveObserve(object sender, ExecutedRoutedEventArgs e)`

Drops the source link the pressed chip names.

### `private void PEtymologyEntryObserve(object sender, ExecutedRoutedEventArgs e)`

Opens the entry a chip names, as the reading view does.

### `private void PEtymologyLinkRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the menu for the selected words, from the editor's mention read.
The pick is heard by `PProspectPickObserve`, which reads the box's selection raw.

### `private void PEtymologyUnlinkObserve(object sender, ExecutedRoutedEventArgs e)`

Drops the span the selection lies inside.

### `private void PEtymologyLinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Linking needs words under the selection, not only spaces.
The mention span verdict owns that blank rule, so the check hands it the raw selection.

### `private void PEtymologyUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Unlinking needs a span the selection lies inside.
