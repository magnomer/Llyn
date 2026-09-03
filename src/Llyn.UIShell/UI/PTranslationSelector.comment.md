# PTranslationSelector.cs

## `internal sealed class PTranslationSelector`

Chooses which of the two shapes a Translation field item is drawn in.
The field holds committed chips and one open entry in a single collection.
So they wrap together as one run of content.
That is only possible if the item template is chosen per item, which is what this does.
