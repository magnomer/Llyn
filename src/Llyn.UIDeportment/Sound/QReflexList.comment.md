# QReflexList.cs
Hash: `fb8f9da6b6cbf7c1`

## `internal sealed class QReflexList`

The reflex rows of one list, shared by the editor's reflex block and the reading view's reflex section.
It owns the rows and the hinge, so both panes reconcile, anchor, lead and fold their rows alike.
It holds no session and asks nothing, since every value comes ready from Conduct.

## `internal QReflexList(ItemsControl list, ToggleButton hinge)`

Binds `list` to the rows and attaches `QReflexItem.QReflexItemRefine`, which fills each row.
The list paints the hinge but does not subscribe its clicks.

## `internal event Action<QReflexItem, CReflexField, string>? QReflexListTyped;`

Passes on every row's `QReflexItemTyped`, with the row, the cell and the text typed.
The editor hears it and hands the edit to its gate, and the reading view leaves it unheard.

## `internal void QReflexListShow(IReadOnlyList<CReflex> rows, bool foldable)`

Brings the rows up to the ready reflexes through `QLookItem.QLookItemShow`, matched by id.
The hinge is visible only when `foldable`, the verdict Conduct readies beside the rows.
A kept row is refined in place, so a row still being typed into keeps its editor.

## `internal void QReflexLeadRefine(IReadOnlyList<CReflexHead> heads)`

Pairs each row with the answer `CKindred.CKindredSet` gave for its id and copies its lead.
The editor calls it while a language is typed, before the draft brings the marks back.

## `internal void QReflexAnchorRefine(CLecternAnchor anchor)`

Writes each row's anchor label by its id, and whether the row may be anchored at all.
Both panes apply ready anchor labels through this method.
A row the labels do not name shows no anchor.

## `internal void QReflexFoldRefine(bool opened)`

Hides the rows through `QReflexHiddenRefine`, and shows the opened state on the hinge.

## `private static void QReflexHiddenRefine(IEnumerable<QReflexItem> rows, bool opened)`

Hides each row its reflex's `CReflexHiddenCheck` hides.
Only the entry page uses it, since the series page folds a member's rows as one block.

## `private QReflexItem QReflexRowRefine(CReflex reflex)`

A row for a ready reflex, whose typed cells pass on through `QReflexListTyped`.

## `private static QReflexItem QReflexStateRefine(QReflexItem row, CReflex reflex)`

Brings a row up to the ready reflex in place, its mark and fold included.
