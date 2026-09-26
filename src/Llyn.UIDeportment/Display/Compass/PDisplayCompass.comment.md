# PDisplayCompass.cs

## `public class PDisplayCompass : ResourceDictionary`

The contents dictionary of the Display panel, loaded from its Veneer markup and merged in.
It holds the Display panel it was built for and forwards a row click to it unchanged.
The panel's handler is internal for this one caller, and hands the click to the lectern's compass.

## `internal PDisplayCompass(PDisplay host)`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.

## `internal void PCompassRowHandle(object sender, RoutedEventArgs e)`

The forwarder the display's row fill subscribes on each realized row.
