# PMentionMenuTemplate.cs

## `public class PMentionMenuTemplate : ResourceDictionary`

The word menu dictionary, loaded from its Veneer markup and merged into the window.
It holds the window it was built for and forwards a row click to it unchanged.

## `private readonly PWindow _pMentionHost`

The window that owns the popup and the choice it reports.

## `internal PMentionMenuTemplate(PWindow host)`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.

## `internal void PMentionMenuHandle(object sender, MouseButtonEventArgs e)`

The forwarder the window's row fill subscribes on each realized row.
