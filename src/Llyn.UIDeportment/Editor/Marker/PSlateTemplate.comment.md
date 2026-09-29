# PSlateTemplate.cs

## `public class PSlateTemplate : ResourceDictionary`

This dictionary displays slate content while the editor decides how it affects the current card.
The editor's fill subscribes its own handlers on each realized row, so the dictionary keeps no host.

## `internal PSlateTemplate()`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.
