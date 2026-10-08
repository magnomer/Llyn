# QReflexList.cs
Hash: `8430fa1abe5b611b`

## `internal sealed class QReflexList`

The reflex rows of one list, shared by the editor's reflex block and the reading view's reflex section.
It owns the rows and the fold toggle, so both panes reconcile, anchor, lead and fold their rows alike.
It holds no session and asks nothing, since every value comes ready from Conduct.

## `internal QReflexList(ItemsControl list, ToggleButton fold)`

Binds `list` to the rows and attaches `QReflexItem.QReflexItemRefine`, which fills each row.
The toggle is only written here, so its owner subscribes its events.

## `internal event Action<QReflexItem, CReflexField, string>? QReflexListTyped;`

Passes on every row's `QReflexItemTyped`, with the row, the cell and the text typed.
The editor hears it and hands the edit to its gate, and the reading view leaves it unheard.

## `internal void QReflexListShow(IReadOnlyList<CReflex> rows)`

Brings the rows up to the ready reflexes through `QLookItem.QLookItemShow`, matched by id.
A kept row is refined in place, so a row still being typed into keeps its editor.

## `internal void QReflexLeadRefine(IReadOnlyList<CReflexHead> heads)`

Pairs each row with the answer `CKindred.CKindredSet` gave for its id and copies its lead.
The editor calls it while a language is typed, before the draft brings the marks back.

## `internal void QReflexAnchorRefine(CLecternAnchor anchor)`

Writes each row's anchor label by its id, and whether the row may be anchored at all.
It is the one writer of both, so the reading view's ready labels and the editor's land alike.
A row the labels do not name shows no anchor.

## `internal void QReflexFoldRefine(bool opened)`

Hides each row its reflex's `CReflexHiddenCheck` hides, and shows the fold state on the toggle.
The toggle is visible only when some row is folded.

## `private QReflexItem QReflexRowRefine(CReflex reflex)`

A row for a ready reflex, whose typed cells pass on through `QReflexListTyped`.

## `private static QReflexItem QReflexStateRefine(QReflexItem row, CReflex reflex)`

Brings a row up to the ready reflex in place, its mark and fold included.
