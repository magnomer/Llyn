# QCadence.cs
Hash: `be08547155a85c98`
Hash: `788974b77e69128d`

## `internal sealed class QCadence`

The editor's paradigm, fanqie, script and reading line use ready blocks from CSounding.
Timbre events trigger redraws but do not supply those blocks.
It finds its controls through `QContract.QContractFind` on the editor, keeping their `PEditor` markup names.
Each panel paints one ready block its sounding read answers, font first, then its rows.

## `internal void QCadenceIntroduce(CDesk desk, CTimbre timbre, CSounding sounding, CFold fold, CLedger ledger, CEnvoy envoy)`

Holds the sounding and fold facets for every read and gate.
The desk and the timbre are only heard, so they are not held.
The script panel's failure notice shows through `ledger` and `envoy` under `Display.ScriptFailed`.
Each panel answers a desk tenure start and its own change event, one subscriber each.
Both panels' renewal notices and the fanqie panel's diwei and representative notices are wired once, to their observers.
Both panels' fold notices go straight to the fold's toggle gates, since each hands one raw value on.
The fold paint answers a tenure start and the fold's change event.

## `private void QCadenceFoldRefine()`

Paints both panels' open switches and bodies from the open states the fold remembers.

## `private void QCadenceParadigmRefine()`

Paints the paradigm panel from the editor's paradigm block, font first.
It maps the slot rows and the inflection view into Deportment items, as the reading view does.
The sheet is null for a pack without a layout or a draft without an entry.

## `private void QCadenceScriptRefine()`

Scans the rows with the panel's `QScriptFailureRefine`, so a picture that fails to decode raises the panel's failure notice.
It also hands the block's verdict on whether the rows may be fetched again.
The panel shows its refresh button from that flag, and `QCadenceFanqieRefine` does the same for the rime books.

## `private void QCadenceFanqieRefine()`

It paints the rime-book font, the rows, the pending flag and the renewable flag from one ready block.

## `private void QCadenceDiweiObserve(bool initial, string key)`

Hands the pressed rime cell's initial flag and key to the sounding, which opens it in the draft's language.

## `private void QCadenceRepresentativeObserve(long fanqieId, int rank, bool raise)`

Hands the pressed row's id, its held rank and the raise flag to the sounding, which stores the new rank.

## `internal void QCadenceReadingRefine(string headword)`

The representative reading of the headword, rewritten with each draft and each rime-book update.
The headword box belongs to the editor, so its text arrives as a parameter.
Its markup style hides the line on empty text, so nothing ranked leaves the header as it was.
