# PContextSelector.cs

## `internal sealed class PContextSelector`

Chooses which of the two shapes a Situation field item is drawn in.
The field holds committed Situations and one open entry in a single collection.
So they wrap together as one run of content.
That is only possible if the item template is chosen per item, which is what this does.
