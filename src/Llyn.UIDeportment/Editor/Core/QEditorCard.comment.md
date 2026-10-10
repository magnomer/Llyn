# QEditorCard.cs
Hash: `cd2dfa32d33dc7f6`

## `internal sealed class QEditorCard`

The card half composes field, sentence, media, list and etymology drivers over shared presentation collections.
Both card lists share one renderer, while each field keeps its own Conduct gate.
Gloss language choices use one shared collection.

## `internal QEditorCard(FrameworkElement surface)`

Drivers share one editor surface and retained collections, keeping dynamic card fields within the same scope.
The card driver receives one example filler assembled from sentence, Gloss and citation drivers.

## `internal QSentence QEditorCardSentence { get; }`

The sentence driver is exposed so the editor can order frame and mention redraws around card rendering.

## `internal QProspect QEditorCardProspect { get; }`

The prospect driver is exposed for translation and mention offers beyond the card fields.

## `internal void QEditorCardIntroduce(CAtelier atelier, CEnvoy envoy, QMentionMenu mentionMenu, CEntry entry, CCard card, CSentence sentence, CCardList list, CCardField field, CImage image, CVideo video)`

Drivers receive their required facets rather than the whole editor.
Drag, badge and list drivers share the supplied list facet.
Sentence navigation shares the atelier's mention area and supplied mention menu.

## `internal void QEditorStartRefine()`

A new tenure clears both card lists, preventing presentation items from the former draft from being retained.

## `internal void QEditorMeaningRefine(CEntryDraft draft)`

Ready meaning cards need no per-card link lookup.
Their folded peek derives from definition wording.

## `private void QEditorTextObserve(object sender, TextChangedEventArgs e)`

The routed text handler accepts only Gloss and image row contexts.
Other fields remain with their own drivers.

## `private void QEditorFieldObserve(TextBox box)`

Keyboard focus is required before raw field text reaches a driver.
The row's type selects Gloss or image handling without reading a field name.

## `internal void QEditorCollocationRefine(CEntryDraft draft)`

Collocations share the renderer but derive their folded peek from expression wording.
