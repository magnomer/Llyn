# QEtymologyEditor.cs
Hash: `5c4288f7bf9d2410`

## `internal sealed class QEtymologyEditor`

The editor's driver for the etymology field: the source links, the narrative and its spans.
The field itself draws both sides, and every change goes through one card gate.
It drives the whole field, chips and prose, not only one half of it.
Resolution goes through the prospect menu, as a translation does, and the engine trims the typed word.

## `internal QEtymologyEditor(FrameworkElement surface, QProspect prospect)`

Binds the field's commands on the editor and listens to the narrative box once, when the editor is built.
It binds the mention pick on the field itself, so only a pick anchored at the narrative box reaches it.
Holds the prospect driver that paints the menu of entries a word matches.

## `internal void QEtymologyIntroduce(CCard card, CEntry entry, CAtelier atelier)`

Holds the card and entry facets and the atelier, and repaints the field after each entry draft change.

## `internal QEtymology QEtymologyField`

The field this driver paints, found by its markup ID.

## `internal static void QEtymologyCaretRefine(PEtymon caret)`

Clears the typed word of the caret chip after its pick.

## Inline notes

### `private void QEtymologyRefine(CEntryDraft draft)`

Hands the field the source links the engine resolved, since the draft holds ids alone.
Each link becomes a chip item here, so the field holds no Conduct record.
A link whose target could not be read is left out rather than drawn blank.

### `private void QEtymologyMentionRefine(CEntryDraft _)`

Paints the spans as the ready chips `CCardEtymologyRead` answers, through the field's chip line.
It answers the same draft bulletin, after the narrative and its sources.

### `private void QEtymologyWriteObserve(object sender, TextChangedEventArgs e)`

The narrative is deferred like every other typed field, so one keystroke is not one request.

### `private void QEtymologyAddRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the menu of the entries the typed word matches, through the prospect driver.
The words are read from the gate here, then handed to the menu to paint.
Only the entry's Return key raises the command, so its parameter and source are the entry and its box.
The pick is heard by `QProspectPickObserve`, which finds the entry from the box the menu stands at.

### `private void QEtymologyRemoveObserve(object sender, ExecutedRoutedEventArgs e)`

Drops the source link the pressed chip names.

### `private void QEtymologyEntryObserve(object sender, ExecutedRoutedEventArgs e)`

Opens the entry a chip names, as the reading view does.

### `private void QEtymologyLinkRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the menu for the selected words, through the prospect driver.
The words are read from the gate here, then handed to the menu to paint.
The pick comes back as the mention pick command on the box, heard by `QEtymologyPickObserve`.

### `private void QEtymologyPickObserve(object sender, ExecutedRoutedEventArgs e)`

Links the selected span of the narrative box to the picked Entry.
The box is the command's source, the anchor the menu stood at, and its text and selection are read raw.
The command's parameter is the raw nullable id, and the gate decides what null means.

### `private void QEtymologyUnlinkObserve(object sender, ExecutedRoutedEventArgs e)`

Drops the span the selection lies inside.

### `private void QEtymologyLinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Linking needs words under the selection, not only spaces.
The mention span verdict owns that blank rule, so the check hands it the raw selection.

### `private void QEtymologyUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Unlinking needs a span the selection lies inside.
