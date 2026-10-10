# QReflex.cs
Hash: `fced449836fc5563`

## `internal sealed class QReflex`

The reflex rows of the input panel, under the headword at the head of the reading stack.
The rows are the same rows the reading view prints, with bare fields where it prints text.
The romanization, meaning and note each have a field after the reading.
The region is only a hover on the language field.
The folded languages hide under the hinge `PReflexHinge`, opened or closed as the engine stores it for the held entry.
Reflex edit requests go through `CKindred`.
Draft refreshes reconcile rows by id, and typed answers update retained rows in place.

## `internal void QReflexIntroduce(CDesk desk, CEntry entry, CKindred kindred, CSounding sounding)`

Holds the kindred facet, the only one a later member reads.
It subscribes the draft, desk start, sounding and reflex changes.
A stored opened state comes back through the fold bulletin, which repaints the draft.

## `private void QReflexAddObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the row the plus was pressed on to `CKindred.CKindredAdd`, or null for the header's plus.
Null is Conduct's word for no row, so no magic id is sent.
The clerk places the new row and gives it the pressed row's language and kind.

## `private void QReflexRemoveObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the row the minus was pressed on to `CKindred.CKindredRemove`.

## `private void QReflexMainObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the row the star was pressed on to `CKindred.CKindredToggle`, which flips its main mark.

## `private void QReflexRebuildRefine(object sender, ExecutedRoutedEventArgs e)`

Holds the table at its width and height before the fetch-again gate runs, bound first on the same command.
So the button stays where it was pressed while the rows are fetched.

## `private void QReflexRebuildObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the fetch-again press to `CKindred.CKindredRebuild`, then repaints the fetching line.
The gate does nothing for a draft not yet stored, and the repaint then lets the table size go.

## `private void QReflexTypeObserve(QReflexItem row, CReflexField field, string text)`

Hears a row's typed cell through `QReflexList.QReflexListTyped` and hands it to `CKindred.CKindredSet`.
It then hands the answer's row to the row as it stands, so it replays no Conduct rule.
The list takes the lead marks of every row from the same answer.

## `private void QReflexRefine(CEntryDraft _)`

Answers the draft bulletin by painting the ready block `CKindred.CKindredRead` answers.
The bulletin's draft is not read, since the block comes whole from Conduct.

## `private void QReflexRefine(CTimbreReflex reflex)`

Has the list bring its rows up to the ready block, keeping a row that is still being typed into.
The stack and the fetch-again button show and hide together, by the block's shown verdict.
The opened state, the anchor labels and the fetching line are painted after the rows.

## `private void QReflexAnchorRefine()`

Answers a desk start and a fanqie change by painting the anchor labels of a fresh block read.

## `private void QReflexAnchorRefine(CLecternAnchor anchor)`

Has the list write every row's ready anchor label and whether a row may be anchored at all.

## `private void QReflexPendingRefine()`

Answers the reflex bulletin by painting the fetching line from `CKindred.CKindredPending`.

## `private void QReflexPendingRefine(bool pending)`

Shows the fetching line and turns the fetch-again icon while the engine fills the entry.
The icon turns while the button carries the `Pending` cue, through the rebuild style's rows.
The held table size is let go once the fill is over, so the new rows size the table again.

## `private void QReflexHingeObserve(object sender, RoutedEventArgs e)`

Hears the hinge's click and hands its raw state to the gate `CKindred.CKindredSpread`.
It hands the gate's verdict to `QReflexHingeRefine`.

## `private void QReflexHingeRefine(bool stored)`

Puts the hinge back to its previous state when the gate refused the write.
A stored write repaints through the fold bulletin, so it paints nothing then.

## `internal QReflex(FrameworkElement surface, QAnchor anchor)`

Builds the `QReflexList` over the reflex list and the hinge, and hears its typed cells.
It attaches the list's quill and control behaviour.
It also binds the row commands on the sound panel.
The anchor command is heard by the anchor menu's Observe on `QAnchor`.
The fetch-again command is bound on its own button, as the markup had it.
The hinge is heard on its click alone, so a repaint that sets it writes nothing back.
The fetch-again command runs its Refine before its Observe, in binding order.
