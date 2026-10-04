# PMentionArgument.cs
Hash: `f1a455172772c9e2`

## `public sealed class PMentionArgument : RoutedEventArgs`

A click on a shown sentence carries surface values over the whole text the control shows.
They are that text and the UTF-16 unit under the pointer in it.
The control converts nothing, so the driver hands these raw values to its gate.
The gate turns them into a code-point offset inside Conduct.
The sentence row is read through to the control that raised the event.
So a host names the row to its gate without naming the control's members.

## `internal PMentionArgument(RoutedEvent routed, PMention origin, string text, int unit)`

Only `PMention` builds it, so the origin cast never fails.

## `public string PMentionArgumentText { get; }`

The whole text the control shows, its runs joined in order, which is the text the gate searches.

## `public int PMentionArgumentUnit { get; }`

The UTF-16 unit under the pointer, counted from the start of the whole text.

## `public PMention PMentionArgumentOrigin => (PMention)OriginalSource;`

The control that raised the click, which the window anchors a choice menu to.

## `public long PMentionArgumentSentence => PMentionArgumentOrigin.PMentionSentence;`

The sentence row the control shows, so a host names the row to its gate without naming the control.
The control owns the row, so the argument reads it there and holds no copy that could drift.
