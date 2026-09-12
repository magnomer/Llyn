# LReceiverRespelling.cs

## `public sealed class LReceiverRespelling : LReceiver`

Wraps a receiver so every candidate reaching it has passed through the pack's respelling groups.
The source start and the finish are forwarded untouched, because respelling changes only the text.
A candidate with a phonetic is copied with the recast text and every other field kept.
A candidate without a phonetic is forwarded as it is, so a broken source still names its row.
The group list may be empty, and the wrapper then forwards the text unchanged.
Whether to wrap at all is the engine's decision, read from the user's switch.
The engine wraps the receiver and never the cache, so a cached lookup replays through this wrapper too.
That is what lets the switch flip without a second fetch and without touching what is stored.

## `public LReceiverRespelling(LReceiver inner, IReadOnlyList<LRespelling> groups)`

Both arguments are required, since a wrapper with nothing to forward to is a caller mistake.
