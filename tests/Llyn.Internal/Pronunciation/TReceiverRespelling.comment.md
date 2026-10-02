# TReceiverRespelling.cs
Hash: `bfa1fef6048ff512`

## `public sealed class TReceiverRespelling`

Covers the wrapper that recasts candidates on their way to a receiver.
A candidate with text reaches the inner receiver with its respelling filled and its phonetic unchanged.
A candidate without text is forwarded as the same object.
A group scoped to one variety leaves a candidate of another variety to the unscoped groups alone.
The source start and the finish each reach the inner receiver exactly once.
