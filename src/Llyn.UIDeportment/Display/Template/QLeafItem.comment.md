# QLeafItem.cs
Hash: `aae51d240483e161`

## `internal sealed class QLeafItem`

Presentation item for one meaning or collocation card of the reading view.
It carries plain copies of what the card template prints, taken from the ready [CLeaf](../../../Llyn.Conduct/Display/CLeaf.comment.md).
The texts are looked up under the keys Conduct chose, so the fills only paint.

## `internal QLeafItem(CLeaf card, CStateWording peek)`

Builds the item from one ready card.
`peek` supplies the definition for a meaning card or the expression for a collocation.
The lectern picks it by the list the card comes in, so the item derives nothing.
A worded text shows its key's looked-up text, and any other text shows as written.
A muted title leaves the kind caption in its place, and a muted line folds away.
Each chip and link becomes its own item.
The example lines become [QLeafLine](QLeafLine.comment.md) items, so the line fill holds no Conduct record.
The picture and film rows become [QLeafImage](QLeafImage.comment.md) and [QLeafVideo](QLeafVideo.comment.md) items for the same reason.

## `internal long QLeafItemId`

The card's id, handed back unread to the fold gate when its hinge is clicked.

## `internal string QLeafItemPeek { get; }`

The looked-up peek text, shown in the title's place while the card is folded and untitled.

## `internal bool QLeafItemFolded`

Whether the card is folded, as the store holds it.

## `internal bool QLeafItemStored`

Whether the card can fold, which decides whether its hinge shows.

## `internal string QLeafItemRank`

The badge formats the supplied position in the current culture, without counting the list.

## `internal string QLeafItemTitle { get; }`

The title key is resolved once when this presentation item is built.

## `internal bool QLeafItemTitled`

An unmuted title occupies the header instead of the kind caption or folded peek.

## `internal string QLeafItemExpression { get; }`

The expression is ready display text, so the fill performs no wording lookup.

## `internal bool QLeafItemExpressed`

The expression line follows Conduct's muted verdict rather than testing display text.

## `internal string QLeafItemMeaning { get; }`

The meaning is ready display text, independent of the folded header's selected peek.

## `internal bool QLeafItemDefined`

The meaning line follows Conduct's muted verdict rather than testing display text.

## `internal IReadOnlyList<QLeafChip> QLeafItemSituation { get; }`

Situation chips retain the supplied card order as presentation items.

## `internal IReadOnlyList<QLeafChip> QLeafItemRegister { get; }`

Register chips retain the supplied card order as presentation items.

## `internal IReadOnlyList<QLeafChip> QLeafItemTag { get; }`

Tag chips retain the supplied card order as presentation items.

## `internal IReadOnlyList<QLinkChip> QLeafItemTranslation { get; }`

Link items retain target identity, headword and language for display and navigation.

## `internal IReadOnlyList<QLeafLine> QLeafItemSentence { get; }`

Example lines use their own presentation items, keeping Conduct records out of template fills.

## `internal IReadOnlyList<QLeafImage> QLeafItemImage { get; }`

Image rows use shared presentation items rather than engine records.

## `internal IReadOnlyList<QLeafVideo> QLeafItemVideo { get; }`

Video rows use shared presentation items rather than engine records.
