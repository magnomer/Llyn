# PDisplayEtymology.cs

## `public partial class PDisplay`

The reading view's half of the etymology: the source links and the narrative, drawn but never edited.

## Inline notes

### `private void PDisplayEtymologyShow(LEntryDraft draft)`

Hands the field the chips already named, and hides the section for an entry that says nothing.
A read the engine refuses leaves the chips empty rather than stopping the page.

### `private void PDisplayEtymologyHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the entry a chip names, as a translation link does.
