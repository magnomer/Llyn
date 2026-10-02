# QLeafItem.cs

## `internal sealed class QLeafItem`

Presentation item for one meaning or collocation card of the reading view.
It carries plain copies of what the card template prints, taken from the ready [CLeaf](../../../Llyn.Conduct/Display/CLeaf.comment.md).
The texts are looked up under the keys Conduct chose, so the fills only paint.

## `internal QLeafItem(CLeaf card)`

Builds the item from one ready card.
A worded text shows its key's looked-up text, and any other text shows as written.
A muted title leaves the kind caption in its place, and a muted line folds away.
Each chip and link becomes its own item.
The example lines become [QLeafLine](QLeafLine.comment.md) items, so the line fill holds no Conduct record.
The picture and film rows become [QLeafImage](QLeafImage.comment.md) and [QLeafVideo](QLeafVideo.comment.md) items for the same reason.
