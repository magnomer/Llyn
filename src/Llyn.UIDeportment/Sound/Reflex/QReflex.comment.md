# QReflex.cs
Hash: `2824189e8841c079`

## `internal sealed class QReflex`

The reflex rows of the input panel, under the headword at the head of the reading stack.
The rows are the same rows the reading view prints, with bare fields where it prints text.
The romanization, meaning and note each have a field after the reading.
The region is only a hover on the language field.
The folded languages hide under the same fold the view has, and the fold state is shared with it.
Every change goes through a `CTimbre` gate, and the rows are rebuilt from the draft it answers with.

## `internal void QReflexIntroduce(CEditor editor)`

Holds the Conduct editor and subscribes the draft, desk start, sounding, reflex and fold changes.

## `private void QReflexAddObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the row the plus was pressed on to `CTimbre.CTimbreReflexAdd`, or zero for the header's plus.
The clerk places the new row and gives it the pressed row's language and kind.

## `private void QReflexRemoveObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the row the minus was pressed on to `CTimbre.CTimbreReflexRemove`.

## `private void QReflexMainObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the row the star was pressed on to `CTimbre.CTimbreReflexToggle`, which flips its main mark.

## `private void QReflexRebuildRefine(object sender, ExecutedRoutedEventArgs e)`

Holds the table at its width and height before the fetch-again gate runs, bound first on the same command.
So the button stays where it was pressed while the rows are fetched.

## `private void QReflexRebuildObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the fetch-again press to `CTimbre.CTimbreReflexRebuild`, then repaints the fetching line.
The gate does nothing for a draft not yet stored, and the repaint then lets the table size go.

## `private void QReflexRefine(CEntryDraft _)`

Answers the draft bulletin by painting the ready block `CTimbre.CTimbreReflexRead` answers.
The bulletin's draft is not read, since the block comes whole from Conduct.

## `private void QReflexRefine(CTimbreReflex reflex)`

Rebuilds the rows from the ready block, keeping a row that is still being typed into.
The stack and the fetch-again button show and hide together, by the block's shown verdict.
The fold, the anchor labels and the fetching line are painted after the rows.

## `private void QReflexAnchorRefine()`

Answers a desk start and a fanqie change by painting the anchor labels of a fresh block read.

## `private void QReflexAnchorRefine(CLecternAnchor anchor)`

Writes every row's ready anchor label and whether a row may be anchored at all.

## `private void QReflexPendingRefine()`

Answers the reflex bulletin by painting the fetching line from `CTimbre.CTimbreReflexPending`.

## `private void QReflexPendingRefine(bool pending)`

Shows the fetching line and turns the fetch-again icon while the engine fills the entry.
The icon turns while the button carries the `Pending` cue, through the rebuild style's rows.
The held table size is let go once the fill is over, so the new rows size the table again.

## `private void QReflexFoldObserve(object sender, RoutedEventArgs e)`

Hands the fold toggle's state to the display's fold gate `CDisplaySound.CDisplayReflexToggle`.

## `private void QReflexFoldRefine()`

Answers the display's fold change by hiding the folded rows and setting the toggle.
The same change redraws every reading view over the same display.

## `private QReflexItem QReflexRowRefine(CReflex reflex)`

A row for a ready reflex, listened to for changes.

## `private static QReflexItem QReflexStateRefine(QReflexItem row, CReflex reflex)`

Brings a row up to the ready reflex in place, its mark and fold included.

## `internal QReflex(FrameworkElement surface, QAnchor anchor)`

Binds the reflex list to its rows and attaches its look, quill and control behaviour.
It also binds the row commands on the sound panel.
The anchor command is heard by the anchor menu's Observe on `QAnchor`.
The fetch-again command is bound on its own button, as the markup had it.
The fold switch is subscribed both ways, since one handler reads its state.
The fetch-again command runs its Refine before its Observe, in binding order.
