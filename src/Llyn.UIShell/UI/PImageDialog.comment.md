# PImageDialog.cs

## `public partial class PEditor`

What the Image rows of a card ask the editor for.
That is a row opened, a row dropped, and a file chosen from this machine.
The rows sit in a template, so their events arrive here rather than at the card.
The card the row belongs to is found by asking which card holds it.

## `internal void PImageAttach(PCard card)`

Points the card's location changes at this editor, so each becomes a deferred request for its row.

## `internal void PImageAddHandle(object sender, RoutedEventArgs e)`

Opens a picture row on the card the Extra row belongs to.

## `internal void PImageRemoveHandle(object sender, RoutedEventArgs e)`

Drops the row from the card that holds it.

## `internal void PImageOpenHandle(object sender, RoutedEventArgs e)`

Chooses a picture file and writes its path into the row.
Browsing is a convenience for the local case only.
The field takes a web address just as well.
The path chosen here is written as it stands rather than copied anywhere.

## `private bool PImagePendingCheck(PCard card, PImage row)`

Whether a location request for the row is still waiting, in which case a redraw must not overwrite it.
