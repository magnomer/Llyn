# PAnchorItem.cs

## `internal sealed class PAnchorItem`

One row of the anchor dropdown: a fanqie row of the character, its label and its tick.
Also the formatting the reflex rows print their anchors with, in the editor and the reading view alike.

## `private const string PAnchorItemKey`

The localization key the tone class is printed through, shared with the rime-book box.

## `private const string PAnchorItemSeparator`

What stands between two anchored placements on one reflex row.

## `internal static IReadOnlyList<PAnchorItem> PAnchorItemScan(`

One item per stored fanqie row, ticked when `anchors` holds its id.
A row the archive has not kept yet has no id and is skipped.

## `internal static string PAnchorTextFormat(IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors)`

The derived readings of the anchored rows in slashes, each distinct reading once, in the order the rows are stored.
Two books giving the same `/lanh/` print it once, since the row answers one sound.
A row the hypothesis gave no reading for prints its full label instead.
Empty when none is anchored.

## `internal static string PAnchorLabelFormat(LFanqieRow row)`

The label of one placement: book, derived reading in slashes, tone class, initial, then the rime cell.
The rime cell is the Diwei rime key, so `寒 I W` reads as the page it opens.
A row the source gave no parts for prints its line instead.
An empty part is left out.
