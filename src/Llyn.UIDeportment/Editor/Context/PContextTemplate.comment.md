# PContextTemplate.cs

## `public class PContextTemplate : ResourceDictionary`

This dictionary keeps context chips visually separate while the editor owns their state and keyboard behavior.
It holds resources only, and the editor's fill subscribes the editor's own methods on each realized part.

## `internal PContextTemplate()`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.
