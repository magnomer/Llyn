# QCadence.cs

## `internal sealed class QCadence`

The editor's sound panels read from the entry's timbre: the paradigm, the fanqie, the script, and the reading line.
It finds its controls through `QContract.QContractFind` on the editor, keeping their `PEditor` markup names.
Each panel takes its font from the window's deportment before its rows are set.

## `internal void QCadenceAttach(PWindow host, LEditor editor)`

Takes the window for fonts and the diwei notice, and the editor for every timbre read.

## `internal void QCadenceFanqieUpdate()`

Rewrites the fanqie panel and hands it its notices.
The editor moves the reflex anchor first and the reading line after, since both follow the same bulletin.

## `internal void QCadenceReadingShow(string headword)`

The representative reading of the headword, rewritten with each draft and each rime-book update.
The headword box belongs to the editor, so its text arrives as a parameter.
Its markup style hides the line on empty text, so nothing ranked leaves the header as it was.
