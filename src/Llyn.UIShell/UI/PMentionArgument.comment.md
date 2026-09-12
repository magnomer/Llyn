# PMentionArgument.cs

## `public sealed class PMentionArgument : RoutedEventArgs`

What a click on a shown sentence carries: the code-point offset clicked and the piece it fell in.
The piece is null when the click landed past every run.
The sentence text, its Mentions and its language are read off the control that raised the event.
So the argument stays small and the host asks the engine with what the control already holds.
