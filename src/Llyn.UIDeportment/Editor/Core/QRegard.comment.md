# QRegard.cs
Hash: `36d9c266748a4e06`

## `internal sealed class QRegard`

The editor's esteem strip: the favourite mark, the grasp steps and their label, and the frequency chip.
It finds its controls through `QContract.QContractFind` on the editor, keeping their `PEditor` markup names.
It holds no draft state, and every write reads the editor deportment's esteem.

## `internal QRegard(FrameworkElement surface)`

Wires the strip's clicks once, right after the editor has taken over its markup's name scope.

## `internal void QRegardIntroduce(CDesk desk, CEsteem esteem)`

Holds the desk, whose stored verdict enables the strip, and the esteem facet the strip reads and writes.
Subscribes the favourite, grasp and frequency notices, and sets how many grasp steps the mark draws.
A tenure start reads every part of the strip again, one subscriber each, since a bulletin comes only on change.

## `private void QRegardHoverRefine(object sender, RoutedEventArgs e)`

The label previews the step under the pointer, so a click never lands on an unread step.

## `private Border QRegardFrequencyChip`

The frequency chip sits under the part-of-speech chips, drawn as the reading view draws it.
It is read only here as there, since a frequency is fetched rather than typed.
A form standing on nothing hides it, and nothing beside it moves when it does.
