# PCategoryTemplate.cs

## `public class PCategoryTemplate : ResourceDictionary`

This dictionary draws the category menu's rows while the editor keeps their behaviour.
The editor's fill subscribes its own category observer on each realized row, so the dictionary keeps no host.

## `internal PCategoryTemplate()`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.
