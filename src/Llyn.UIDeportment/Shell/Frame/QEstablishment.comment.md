# QEstablishment.cs
Hash: `1df74acc85733638`

## `internal sealed class QEstablishment`

Drives the status bar along the foot of the window.
Its left end holds the Joplin control, which `QCourier` drives.
After it, the bar says whether the window holds unsaved work, and its right says how big the workspace is.
It holds nothing of its own and prints what the engine answers.
The bar itself is the veneer's `PEstablishment` page, which the window places.

## `internal QEstablishment(FrameworkElement surface)`

Takes the bar the window pulled under the contract ID `PEstablishment`.
Builds the Joplin driver over the same bar, so the window keeps one driver for it.

## `private readonly QCourier _qEstablishmentCourier;`

The Joplin driver at the bar's left end.
It paints its own part from its own courier, so the status read never carries Joplin state.

## `private Action _qEstablishmentRelease = () => { };`

Detaches the bar's observer, and does nothing until the bar is attached.
The observer stays inside this delegate, so no field holds an engine announcement.

## `private StackPanel QEstablishmentPending =>`

Each named part is pulled from the bar by its contract ID on every read.

## `internal void QEstablishmentAttach(QWindow host)`

Subscribes the bar to `CWorkspaceEstablishmentChanged`, which the workspace raises on every open and every bulletin.
So a change made anywhere reaches the bar without any panel telling it.
The detach is kept for `QEstablishmentClose`.
The observer runs on the window's thread, which is the bar's thread too.
The Joplin driver is introduced last, still before the atelier opens, so its courier hears the first open.

## `internal void QEstablishmentClose()`

Stops listening before the engine goes.
A second close detaches an observer already gone, which the engine ignores.

## `private void QEstablishmentRefine(CEstablishment establishment)`

Prints the three lines the atelier read, each through the wording key it chose.
The unsaved line is hidden when nothing is unsaved, so a calm window shows nothing on the left.
A refused read never arrives here, so the last printed numbers stand.
The singular entry wording and the size unit arrive as ready keys, and the amount as ready text.
