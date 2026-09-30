# QCadence.cs

## `internal sealed class QCadence`

The editor's sound panels read from the entry's timbre: the paradigm, the fanqie, the script, and the reading line.
It finds its controls through `QContract.QContractFind` on the editor, keeping their `PEditor` markup names.
Each panel paints one ready block its sounding read answers, font first, then its rows.

## `internal void QCadenceIntroduce(CEditor editor)`

Takes the editor for every sounding read and gate.
Each panel answers a tenure start and its own change event from the editor, one subscriber each.
The fanqie panel's diwei and representative notices are wired once, to their observers.

## `private void QCadenceScriptRefine()`

Offers the refresh button only when the block says the rows may be fetched again.
The button then calls `QCadenceScriptObserve`, and `QCadenceFanqieRefine` does the same for the rime books.

## `private void QCadenceFanqieRefine()`

The editor moves the reflex anchor and the reading line on the same change event.

## `private void QCadenceDiweiObserve(bool initial, string key)`

Hands the pressed rime cell's initial flag and key to the sounding, which opens it in the draft's language.

## `private void QCadenceRepresentativeObserve(long fanqieId, int rank, bool raise)`

Hands the pressed row's id, its held rank and the raise flag to the sounding, which stores the new rank.

## `internal void QCadenceReadingRefine(string headword)`

The representative reading of the headword, rewritten with each draft and each rime-book update.
The headword box belongs to the editor, so its text arrives as a parameter.
Its markup style hides the line on empty text, so nothing ranked leaves the header as it was.
