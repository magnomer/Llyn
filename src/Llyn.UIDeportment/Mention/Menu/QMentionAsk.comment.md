# QMentionAsk.cs
Hash: `99ebaa29f9e3be85`


## `internal sealed class QMentionAsk`

One open request for the sense menu, answered by `QMentionMenu.QMentionMeaningRefine`.
The area that opened the menu subscribes this ask alone, so it hears only its own pick.
So no area compares anchors to tell its pick from another's.
A new ask stands for every opening, and the menu forgets the old one on every hide.

## `internal QMentionAsk(FrameworkElement anchor)`

Keeps the field the sense menu was opened under.

## `internal event Action<FrameworkElement, long>? QMentionAskChosen`

Raised once a sense row is picked, with the asking field and the sense id.
Zero means the whole Entry.

## `internal FrameworkElement QMentionAskAnchor`

The field the sense menu was opened under, handed back with the pick.

## `internal void QMentionAskSettle(long sense)`

The menu settles the ask with the picked sense, which raises `QMentionAskChosen`.
