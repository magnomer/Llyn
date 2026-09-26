# PClipTemplate.cs

## `public class PClipTemplate : ResourceDictionary`

The recording menu dictionary, loaded from its Veneer markup and merged into the editor.
It holds the editor it was built for and forwards the two row clicks to it unchanged.

## `private readonly PEditor _pClipHost`

The editor that owns the recordings and their preview player.

## `internal PClipTemplate(PEditor host)`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.

## `internal void PClipPreviewHandle(object sender, RoutedEventArgs e)`

The forwarder the editor's reading fill subscribes on each play button.

## `internal void PClipSelectorHandle(object sender, RoutedEventArgs e)`

The forwarder the editor's reading fill subscribes on each taking button.
