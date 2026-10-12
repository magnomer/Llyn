# QBondItem.cs
Hash: `91b81f9342fbd78b`

## `internal sealed class QBondItem`

One linked meaning as a collocation card's chip or as a row of the chooser menu.
It is the Deportment item the surface holds, so no control ever holds a Conduct meaning.
Every value is fixed at construction, so a chip never changes which link it names.

## `private const double QBondItemStep = 16;`

The indent in pixels for each depth level in the chooser menu.
It matches the mention menu's step, so both pickers show the meaning tree alike.

## `internal QBondItem(long card, CMeaning sense)`

The item for one meaning of one card.
The card's id rides along, so the chip's eraser names its card without searching the controls.

## `public long QBondItemCard { get; }`

The id of the card the chip belongs to, handed back unread to the detach gate.

## `public long QBondItemSense { get; }`

The meaning's id, which keys the chip list and is handed back unread to the sense gates.

## `public string QBondItemPath { get; }`

The meaning's outline number, such as `2.1`, shown before its name.

## `public string QBondItemName { get; }`

The meaning's name as Conduct worded it.

## `public Thickness QBondItemIndent { get; }`

The menu row's left margin for the meaning's depth in the tree.

## `internal static IReadOnlyList<QBondItem> QBondItemCreate(long card, IReadOnlyList<CMeaning> senses)`

Maps Conduct's meanings to items of one card, in the order Conduct sent them.
