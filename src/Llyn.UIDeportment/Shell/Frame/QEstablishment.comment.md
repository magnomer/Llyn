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

Puts the bar to work on the one engine and prints the first reading at once.
The bar subscribes like a panel, so a change made anywhere reaches it without any panel telling it.
The observer runs on the window's thread, which is the bar's thread too.
Every bulletin is a reason to read again, whatever its kind.
Sorting the kinds would save one cheap read and would miss the next kind added.

## `internal void QEstablishmentClose()`

Stops listening before the engine goes.
A second close detaches an observer already gone, which the engine ignores.

## `private void QEstablishmentHook(PWindow host, Action<LBulletin> observer)`

Attaches the observer and keeps its detach for the close.

## `private void QEstablishmentUpdate()`

Reads the three numbers and prints them.
The unsaved line is hidden when nothing is unsaved, so a calm window shows nothing on the left.
A refused read leaves the last printed numbers standing, since the bar has no room for a failure.
The entry line picks its singular wording by count, which the locale files spell out.

## `private static string QEstablishmentSizeFormat(long bytes)`

Megabytes with one decimal once the file reaches one, kilobytes rounded up below that.
A workspace of a few words would otherwise print as zero, which reads as a missing file.
