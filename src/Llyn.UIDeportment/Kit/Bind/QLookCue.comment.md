# QLookCue.cs

## `internal enum QLookCue`

The visual states a look-sheet row can wait for, as flags.
A row names every cue it needs, and it holds only while all of them hold, as a multi-trigger did.
The control's own flags give most cues, such as hover, press, focus and disabled.
A view gives the rest through `QLook.QLookCueProperty`, such as chosen, pending or playing.

## `QLookCueBase = 0,`

No cue at all, so a `Base` row holds always.

## `QLookCueBare = 1 << 11,`

The control carries no icon under `QLook.QLookIconProperty`, so its icon part folds away.

## `QLookCueMute = 1 << 12,`

The control has no content, so a label part folds away and only the icon shows.
