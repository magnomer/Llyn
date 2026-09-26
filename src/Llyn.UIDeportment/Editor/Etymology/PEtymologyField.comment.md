# PEtymologyField.cs

## `public partial class PEditor`

The editor's half of the etymology field: the source links, the narrative and its spans.
The field itself draws both sides, and the card deportment builds every request.
Resolution goes through the prospect menu, as a translation does, because the editor holds the engine.

## `internal void PEtymologyAttach()`

Binds the field's commands and listens to the narrative box once, when the editor is built.

## Inline notes

### `private void PEtymologyShow(LEntryDraft draft)`

Hands the field the source links the engine resolved, since the draft holds ids alone.
A link whose target could not be read is left out rather than drawn blank.

### `private void PEtymologyWriteHandle(object sender, TextChangedEventArgs e)`

The narrative is deferred like every other typed field, so one keystroke is not one request.

### `private void PEtymologyAddHandle(object sender, ExecutedRoutedEventArgs e)`

Offers the entries the typed word matches, and links the one the user picks.
Only the entry's Return key raises the command, so its parameter and source are the entry and its box.

### `private void PEtymonSend(PEtymon caret, long entryId)`

Clears the entry before the request, so the word does not stand beside its own chip.
The prospect menu only answers with a stored entry, so no id needs checking here.

### `private void PEtymologyRemoveHandle(object sender, ExecutedRoutedEventArgs e)`

Drops the source link the pressed chip names.

### `private void PEtymologyEntryHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the entry a chip names, as the reading view does.

### `private void PEtymologyLinkHandle(object sender, ExecutedRoutedEventArgs e)`

Links the selected words once an entry is picked.
The selection is read when the menu opens, so a later click cannot move the span.

### `private void PEtymologyUnlinkHandle(object sender, ExecutedRoutedEventArgs e)`

Drops the span the selection lies inside.

### `private void PEtymologyLinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Linking needs words under the selection, not only spaces.

### `private void PEtymologyUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Unlinking needs a span the selection lies inside.
