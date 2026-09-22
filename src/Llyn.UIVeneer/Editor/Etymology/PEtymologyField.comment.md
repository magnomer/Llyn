# PEtymologyField.cs

## `public partial class PEditor`

The editor's half of the etymology field: the source links, the narrative and its spans.
The field itself draws both sides, so only the requests live here.
Resolution goes through the prospect menu, as a translation does, because the editor holds the engine.

## `internal void PEtymologyAttach()`

Binds the field's commands and listens to the narrative box once, when the editor is built.

## Inline notes

### `private void PEtymologyShow(LEntryDraft draft, IReadOnlyDictionary<long, LTranslationTarget> targets)`

Hands the field the chips already named, since the draft holds ids alone.
A link whose target could not be read is left out rather than drawn blank.

### `private void PEtymologyWriteHandle(object sender, TextChangedEventArgs e)`

The narrative is deferred like every other typed field, so one keystroke is not one request.

### `private void PEtymologyAddHandle(object sender, ExecutedRoutedEventArgs e)`

Offers the entries the typed word matches, and links the one the user picks.

### `private void PEtymonSend(PEtymologyCaret caret, long entryId)`

Clears the entry before the request, so the word does not stand beside its own chip.

### `private void PEtymologyRemoveHandle(object sender, ExecutedRoutedEventArgs e)`

Drops the source link the pressed chip names.

### `private void PEtymologyEntryHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the entry a chip names, as the reading view does.

### `private void PEtymologyLinkHandle(object sender, ExecutedRoutedEventArgs e)`

Links the selected words once an entry is picked, the offsets read in code points.

### `private void PEtymologyMentionSend(int offset, int length, long entryId)`

A span of an etymology always names an entry, so an unpicked word sends nothing.

### `private void PEtymologyUnlinkHandle(object sender, ExecutedRoutedEventArgs e)`

Naming no entry over the standing span is how that span is dropped.

### `private void PEtymologyLinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Linking needs words under the caret.

### `private void PEtymologyUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Unlinking needs a span the caret sits inside.

### `private LMentionDraft? PEtymologySpanFind()`

The span the selection falls within, or none.
