# PVideoTemplate.cs

## `public class PVideoTemplate : ResourceDictionary`

The video row dictionary, loaded from its Veneer markup and merged by the entry and situation editors.
It hands the two clicks a row raises back through `PVideoHost` to whichever editor built it.

## `private readonly PVideoHost _pVideoHost`

The editor that owns the videos of its draft.

## `internal PVideoTemplate(PVideoHost host)`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.

## `internal void PVideoOpenHandle(object sender, RoutedEventArgs e)`

The forwarder the host's row fill subscribes on each browse button.

## `internal void PVideoRemoveHandle(object sender, RoutedEventArgs e)`

The forwarder the host's row fill subscribes on each remove handle.
