# PDisplayEtymology.cs

## `public partial class PDisplay`

The reading view's half of the etymology: the source links and the narrative, drawn but never edited.

## Inline notes

### `private void PDisplayEtymologyShow(LEntryDraft draft)`

Hands the field the source links the engine resolved, and hides the section for an entry that is not derived.
A read the engine refuses leaves the links empty rather than stopping the page.

### `private void PDisplayEtymologyApply(LEntryDraft draft, IReadOnlyList<LTranslationTarget> etymons)`

Shows the field only when it has a narrative or a link, as the display deportment decides.

### `private void PDisplayEtymologyHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the entry a chip names, as a translation link does.
