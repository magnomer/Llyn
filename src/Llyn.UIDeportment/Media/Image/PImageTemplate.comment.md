# PImageTemplate.cs

## `public class PImageTemplate : ResourceDictionary`

The picture row dictionary, loaded from its Veneer markup and merged by the entry and situation editors.
It hands the two clicks a row raises back through `PImageHost` to whichever editor built it.

## `private readonly PImageHost _pImageHost`

The editor that owns the pictures of its draft.

## `internal PImageTemplate(PImageHost host)`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.

## `internal void PImageOpenHandle(object sender, RoutedEventArgs e)`

The forwarder the host's row fill subscribes on each browse button.

## `internal void PImageRemoveHandle(object sender, RoutedEventArgs e)`

The forwarder the host's row fill subscribes on each remove handle.
