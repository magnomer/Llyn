# PEditorChange.cs

## `public partial class PEditor`

The two action buttons are only offered while there is work they would act on.
An untouched form has nothing to save and nothing to throw away.
So both stay dim until the form differs from the state it opened in.
The same comparison already answers the closing question, so the buttons cannot disagree with the warning.

## Inline notes

### `private DispatcherTimer? _pEditorClock;`

Work reaches the form from too many places to listen to each one.
Typed fields, dropped cards, dragged images, and downloaded recordings all count as change.
A slow tick asks the form itself instead of trusting every path to report.

### `Interval = TimeSpan.FromMilliseconds(200)`

Fast enough that the buttons wake within one keystroke of the change.
Slow enough that reading the whole form back costs nothing anyone can feel.

### `clock.Tick += (_, _) => PEditorChangeUpdate();`

The tick is the only caller, so the check stays a background concern.
Nothing in the editing path has to remember to keep the buttons honest.
