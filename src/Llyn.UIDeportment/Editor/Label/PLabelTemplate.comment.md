# PLabelTemplate.cs

## `public class PLabelTemplate : ResourceDictionary`

This dictionary shares label chips and editing controls across cards without owning their state.
It holds resources only, and the editor's fill subscribes the editor's own methods on each realized part.

## `internal PLabelTemplate()`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.
