# PMarkerTemplate.cs

## `public class PMarkerTemplate : ResourceDictionary`

This dictionary draws speech markers as editor chips while speech data stays with the editor.
The editor's fill subscribes its own erase observer on each realized chip, so the dictionary keeps no host.

## `internal PMarkerTemplate()`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.
