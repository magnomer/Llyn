# PClipTemplate.cs

## `public class PClipTemplate : ResourceDictionary`

The recording menu dictionary, loaded from its Veneer markup and merged into the editor.
The editor's reading fill subscribes its own play and taking observers, so the dictionary forwards nothing.

## `internal PClipTemplate()`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.
