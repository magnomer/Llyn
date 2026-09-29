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

Opens the menu of the entries the typed word matches, and hands the pick to `PEtymonPickObserve`.
Only the entry's Return key raises the command, so its parameter and source are the entry and its box.

### `private void PEtymonPickObserve(PEtymon caret, long entryId)`

Links the picked entry, then empties the entry so the word does not stand beside its own chip.
The prospect menu only answers with a stored entry, so no id needs checking here.

### `private void PEtymologyRemoveObserve(object sender, ExecutedRoutedEventArgs e)`

Drops the source link the pressed chip names.

### `private void PEtymologyEntryObserve(object sender, ExecutedRoutedEventArgs e)`

Opens the entry a chip names, as the reading view does.

### `private void PEtymologyLinkRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the menu for the selected words, and hands the pick to `PEtymologySpanObserve`.
The selection is read when the menu opens, so a later click cannot move the span.

### `private void PEtymologyUnlinkObserve(object sender, ExecutedRoutedEventArgs e)`

Drops the span the selection lies inside.

### `private void PEtymologyLinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Linking needs words under the selection, not only spaces.
The mention span verdict owns that blank rule, so the check hands it the raw selection.

### `private void PEtymologyUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Unlinking needs a span the selection lies inside.
