# QEstablishment.cs

## `internal sealed class QEstablishment`

Drives the status bar along the foot of the window.
Its left says whether the window holds unsaved work, and its right says how big the workspace is.
It holds nothing of its own and prints what the engine answers.
The bar itself is the veneer's `PEstablishment` page, which the window places.

## `internal QEstablishment(FrameworkElement surface)`

Takes the bar the window pulled under the contract ID `PEstablishment`.

## `private Action _qEstablishmentRelease = () => { };`

Detaches the bar's observer, and does nothing until the bar is attached.
The observer stays inside this delegate, so no field holds an engine announcement.

## `private StackPanel QEstablishmentPending =>`

Each named part is pulled from the bar by its contract ID on every read.

## `internal void QEstablishmentAttach(PWindow host)`

Puts the bar to work with one attach to `CAtelierEstablishmentAttach`, which prints the first reading at once.
The atelier reads again on every bulletin, so a change made anywhere reaches the bar without any panel telling it.
The observer runs on the window's thread, which is the bar's thread too.

## `internal void QEstablishmentClose()`

Stops listening before the engine goes.
A second close detaches an observer already gone, which the engine ignores.

## `private void QEstablishmentRefine(CEstablishment establishment)`

Prints the three lines the atelier read, each through the wording key it chose.
The unsaved line is hidden when nothing is unsaved, so a calm window shows nothing on the left.
A refused read never arrives here, so the last printed numbers stand.
The singular entry wording and the size unit arrive as ready keys, and the amount as ready text.
