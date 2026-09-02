# PImageMenu.cs

## `public partial class PEditor`

What the Image rows of a card ask the editor for.
That is a row opened, a row dropped, and a file chosen from this machine.
The rows sit in a template, so their events arrive here rather than at the card.
The card the row belongs to is found by asking which card holds it.

## `internal void PImageAddHandle(object sender, RoutedEventArgs e)`

Opens a picture row on the card the Extra row belongs to.

## `internal void PImageRemoveHandle(object sender, RoutedEventArgs e)`

Drops the row from the card that holds it.

## `internal void PImageOpenHandle(object sender, RoutedEventArgs e)`

Chooses a picture file and writes its path into the row.
Browsing is a convenience for the local case only.
The field takes a web address just as well.
The path chosen here is written as it stands rather than copied anywhere.
