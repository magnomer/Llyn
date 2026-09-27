# QFootnoteItem.cs

## `internal sealed class QFootnoteItem`

One Entry as a row of the sources panel's entry list.
It mirrors the taxonomy panel's row, because every middle column answers the same shape of question.
That question is which Entries cite the Source chosen in the shelf beside them.
An Entry referencing from several of its cards is still one row, because the row stands for the Entry.
The headword and the shown name are separate, so two Entries sharing a headword can be numbered apart.
The mark saying which row the reader stands on is the one thing that changes after the row is built.

## `public string QFootnoteItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.

## `internal static IReadOnlyList<QFootnoteItem> QFootnoteItemBuild(IReadOnlyList<CVistaRow> rows)`

One item per Conduct row, in the order the shelf returned them.
Each row carries its own chosen mark into the constructor, so the mark has one kind of writer.
A plain copy loop, so the panel that asks for it carries no loop of its own.

## `internal static bool QFootnoteItemMatch(QFootnoteItem held, QFootnoteItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`LSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void QFootnoteItemSync(QFootnoteItem held, QFootnoteItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `LSplice` kept.

## `internal static void QFootnoteItemApply(FrameworkElement container, object item, string? _)`

Fills one entry row with its flag, name, epithet and language, and marks the chosen row.
The epithet is led by an en space, as the other catalog rows set it apart.
