# PSlateTemplate.cs

## `public class PSlateTemplate : ResourceDictionary`

This dictionary displays slate content while the editor decides how it affects the current card.

The host's fill subscribes each forwarder on the realized part, where an event attribute stood before.

## `private readonly PEditor _pSlateHost`

The editor provides the active card context for slate interactions.

## `internal PSlateTemplate(PEditor host)`

The host connects shared slate presentation to the editor that owns its card.
Merges the markup the Veneer holds, since the dictionary carries no class of its own there.

## `internal void PSlateHandle(object sender, MouseButtonEventArgs e)`

A slate click delegates to the editor so it can apply the action to the right card.
