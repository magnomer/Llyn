# PMentionArgument.cs

## `public sealed class PMentionArgument : RoutedEventArgs`

What a click on a shown sentence carries: the offset clicked and what the control held.
The engine finds the Mention at that offset itself, so the argument names no piece.
The sentence text, its Mentions and its language are read through to the control that raised the event.
So a host asks the engine without naming the control's members.

## `public PMention PMentionArgumentOrigin => (PMention)OriginalSource;`

The control that raised the click, which the window anchors a choice menu to.
