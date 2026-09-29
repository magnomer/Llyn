# PLinkTemplate.cs

## `public class PLinkTemplate : ResourceDictionary`

This dictionary presents link chips and their entry while the owning editor manages link state.
It holds resources only, and the editor's fill subscribes the editor's own methods on each realized part.

## `internal PLinkTemplate()`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.
