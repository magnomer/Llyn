# PEstablishment.xaml.cs

## `public partial class PEstablishment : UserControl`

The status bar along the foot of the window.
Its left says whether the window holds unsaved work, and its right says how big the workspace is.
It holds nothing of its own and prints what the engine answers.

## `internal void PEstablishmentAttach(PWindow host)`

Puts the bar to work on the one engine and prints the first reading at once.
The bar subscribes like a panel, so a change made anywhere reaches it without any panel telling it.

## `internal void PEstablishmentClose()`

Stops listening before the engine goes.

## `private void PEstablishmentBulletinHandle(LBulletin bulletin)`

Every bulletin is a reason to read again, whatever its kind.
A draft keystroke moves the unsaved count and a commit moves the entry count.
Any fetch that stores rows moves the size.
Sorting the kinds would save one cheap read and would miss the next kind added.

## `internal void PEstablishmentUpdate()`

Reads the three numbers and prints them.
The unsaved line is hidden when nothing is unsaved, so a calm window shows nothing on the left.
A refused read leaves the last printed numbers standing, since the bar has no room for a failure.
The entry line picks its singular wording by count, which the locale files spell out.

## `private string PEstablishmentSizeFormat(long bytes)`

Megabytes with one decimal once the file reaches one, kilobytes rounded up below that.
A workspace of a few words would otherwise print as zero, which reads as a missing file.
