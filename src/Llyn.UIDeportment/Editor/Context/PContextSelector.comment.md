# PContextSelector.cs
Hash: `1633b5000b4cae23`

## `internal sealed class PContextSelector : DataTemplateSelector`

Chooses which of the two shapes a Situation field item is drawn in.
The field draws committed Situations and one open entry as a single run of items.
So they wrap together as one run of content.
That is only possible if the item template is chosen per item, which is what this does.
