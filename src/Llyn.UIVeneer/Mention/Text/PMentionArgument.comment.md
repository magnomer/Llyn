# PMentionArgument.cs

## `public sealed class PMentionArgument : RoutedEventArgs`

What a click on a shown sentence carries: the code-point offset clicked, and nothing else.
The engine finds the Mention at that offset itself, so the argument names no piece.
The sentence text, its Mentions and its language are read off the control that raised the event.
So the argument stays small and the host asks the engine with what the control already holds.
