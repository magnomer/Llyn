# QLeafItem.cs
Hash: `4a9ff3986e30777d`

## `internal sealed class QLeafItem`

Presentation item for one meaning card or one collocation row of the reading view.
It carries plain copies of what the card template prints, taken from the ready [CLeaf](../../../Llyn.Conduct/Display/CLeaf.comment.md).
The texts are looked up under the keys Conduct chose, so the fills only paint.

## `internal QLeafItem(CLeaf card, CStateWording peek)`

Builds the item from one ready card.
`peek` supplies the definition for a meaning card or the phrase for a collocation.
The lectern card picks it by the list the card comes in.
A worded text shows its key's looked-up text, and any other text shows as written.
A muted title leaves the kind caption in its place, and a muted line folds away.
Each chip and link becomes its own item.
Each collocation homed on a meaning card becomes a nested item, peeking its phrase.
Each "see" line becomes a `QLeafSee` item.
The example lines become [QLeafLine](QLeafLine.comment.md) items, so the line fill holds no Conduct record.
The picture and film rows become [QLeafImage](QLeafImage.comment.md) and [QLeafVideo](QLeafVideo.comment.md) items for the same reason.

## `internal long QLeafItemId`

The card's id, handed back unread to the fold gate when its hinge is clicked.

## `internal string QLeafItemPeek { get; }`

The looked-up peek text, shown in the title's place while the card is folded and untitled.

## `internal bool QLeafItemFolded`

Whether the card is folded, as the store holds it.

## `internal bool QLeafItemStored`

Whether the card is stored, which decides whether its hinge shows.

## `internal string QLeafItemRank`

The badge formats the supplied position in the current culture, without counting the list.

## `internal string QLeafItemTitle { get; }`

The title key is resolved once when this presentation item is built.
A collocation row is built with its phrase as `peek`, since a row has no folded title place.

## `internal bool QLeafItemTitled`

An unmuted title occupies the header instead of the kind caption or folded peek.

## `internal string QLeafItemPhrase { get; }`

The phrase a collocation row is headed with, resolved under Conduct's key once, so the row fill only paints.

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

Image rows use shared presentation items rather than Conduct records.

## `internal IReadOnlyList<QLeafVideo> QLeafItemVideo { get; }`

Video rows use shared presentation items rather than Conduct records.

## `internal IReadOnlyList<ImageSource?> QLeafItemFlag { get; }`

One ensign per translation language of a closed row, mapped from the language once when the item is built.
A language without an ensign gives an empty image, so the row never filters Conduct's list.

## `internal string QLeafItemExample`

The closed row's ready "N examples" marker, Conduct's text unchanged.

## `internal bool QLeafItemQuoted`

The example marker follows Conduct's verdict on whether the row has one, rather than measuring the text.

## `internal string QLeafItemAlso`

The closed row's ready "also N" marker, Conduct's text unchanged.

## `internal bool QLeafItemShared`

The "also N" marker follows Conduct's verdict on whether the row has one, rather than measuring the text.

## `internal string QLeafItemHeading`

The ready "Collocations · N" caption over a meaning's rows, Conduct's text unchanged.

## `internal IReadOnlyList<QLeafItem> QLeafItemHome { get; }`

The collocations homed on this meaning or one below it, shown as folding rows beneath it.
A collocation item holds none, since the home rule gives a collocation no collocations.

## `internal IReadOnlyList<QLeafSee> QLeafItemSee { get; }`

The "see" lines for collocations that also link here or below but live under another meaning.
Each is its own item, carrying the collocation's id for the click and Conduct's text unchanged.
