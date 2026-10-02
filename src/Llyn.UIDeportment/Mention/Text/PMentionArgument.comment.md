# PMentionArgument.cs
Hash: `399bd35862050e54`

## `public sealed class PMentionArgument : RoutedEventArgs`

A click on a shown sentence carries the offset clicked and what the control held.
The engine finds the Mention at that offset itself, so the argument names no piece.
The sentence row is read through to the control that raised the event.
So a host names the row to its gate without naming the control's members.

## `internal PMentionArgument(RoutedEvent routed, PMention origin, int offset)`

Only `PMention` builds it, so the origin cast never fails.

## `public int PMentionArgumentOffset { get; }`

A code-point offset into the whole sentence, not a UTF-16 index.
It is the offset the engine reads a Mention at.

## `public PMention PMentionArgumentOrigin => (PMention)OriginalSource;`

The control that raised the click, which the window anchors a choice menu to.
