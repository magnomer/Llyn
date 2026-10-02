# QLookCue.cs
Hash: `6b9a3fd8e550a325`

## `internal enum QLookCue`

The visual states a look-sheet row can wait for, as flags.
A row names every cue it needs, and it holds only while all of them hold, as a multi-trigger did.
The control's own flags give most cues, such as hover, press, focus and disabled.
A view gives the rest through `QLook.QLookCueProperty`, such as chosen, pending or playing.

## `QLookCueBase = 0,`

No cue at all, so a `Base` row holds always.

## `QLookCueEmpty = 1 << 6,`

The control shows nothing, read per kind of control.
An empty text, a choice with no text, an image with no source, or a list without items all count.
A choice box with no items does not, since its text decides.

## `QLookCueSelected = 1 << 9,`

A list item reports it itself, and a view may also give it.
The editor's tab stack gives it to the tab whose page is showing.

## `QLookCueBare = 1 << 11,`

The control carries no icon under `QLook.QLookIconProperty`, so its icon part folds away.

## `QLookCueMute = 1 << 12,`

The control has no content, so a label part folds away and only the icon shows.

## `QLookCueIdle = 1 << 14,`

Given by a view to a tab whose page is hidden, the opposite of `QLookCueSelected`.
A hover row waits on it too, so the shown tab keeps its look under the pointer.

## `QLookCueMarked = 1 << 16,`

A fanqie line marked as the primary representative, so its star and order show at full strength.

## `QLookCueFaded = 1 << 17,`

A fanqie line that is marked but is not the primary, so the line is dimmed.
An unmarked line takes `QLookCueBase` instead.
