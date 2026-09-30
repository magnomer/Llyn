# PDisplayCompass.cs

## `public class PDisplayCompass : ResourceDictionary`

The contents dictionary of the Display panel, loaded from its Veneer markup and merged in.
It holds the lectern's compass and hands it a row click unchanged.

## `internal PDisplayCompass()`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.

## `internal void PCompassIntroduce(QCompass driver)`

Receives the lectern's compass once the display is attached.

## `internal void PCompassRowRefine(object sender, RoutedEventArgs e)`

The bridge the display's row fill subscribes on each realized row.
It hands the click to the compass, which scrolls to the section the row names.
