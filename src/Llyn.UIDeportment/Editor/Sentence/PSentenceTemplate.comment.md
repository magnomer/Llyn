# PSentenceTemplate.cs

## `public class PSentenceTemplate : ResourceDictionary`

This dictionary provides sentence examples and editing commands while the editor owns sentence state.
The editor's fill subscribes its own handlers on each realized row, so the dictionary keeps no host.

## `internal PSentenceTemplate()`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.
