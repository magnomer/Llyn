# QStemItem.cs
Hash: `6bddfb5ad0f18164`

## `internal sealed class QStemItem`

One member row of the series page: the character, its reading line and its reflex rows.
It copies the Conduct member, so the member list never hands a Conduct record to a control.

## `private QStemItem(CStemMember member)`

Copies one member and wraps each reflex row as the entry page's `QReflexItem`.
No row is hidden one by one, since the series fold collapses the whole block.

## `public string QStemItemCharacter`

The member character, which the chip prints and hands to the entry command.

## `public string QStemItemReading`

The representative line of the character, empty when no reading is marked.

## `public IReadOnlyList<QReflexItem> QStemItemReflexes`

The reflex rows, read-only, with no anchor or spinner, since the series page wires neither.

## `public bool QStemItemOpened`

Whether the member's reflex rows are shown, which the hinge shows as checked.

## `public bool QStemItemFoldable`

Whether a hinge applies to the member at all, as Conduct decides.

## `internal static IReadOnlyList<QStemItem> QStemItemBuild(IReadOnlyList<CStemMember> members)`

Copies a page's members, in order, into rows the member list can show.

## `internal static void QStemItemRefine(FrameworkElement container, object item, string? _)`

Fills one member row: the character on its chip, the reading line beside it, and its reflex rows below.
The reflex rows are filled by the same `QReflexItem.QReflexItemRefine` the entry page uses.
A member with no reading or no reflex row leaves that part empty.
The `QLook` Empty row of `Theme.Text.Reading` collapses the empty reading.
The reflex rows show only while the member is opened, and collapse as one block while folded.
The hinge shows the opened state and appears only on a foldable member.
