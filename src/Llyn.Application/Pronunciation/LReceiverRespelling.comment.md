# LReceiverRespelling.cs

## `public sealed class LReceiverRespelling : LReceiver`

Wraps a receiver so every candidate reaching it carries its phonetic recast through the pack's respelling groups.
The source start and the finish are forwarded untouched, because respelling adds only a text.
A candidate with a phonetic is copied with the recast text in its respelling and every other field kept.
The original phonetic stays as it came, so the menu shows either form and the pick stores both.
A candidate without a phonetic is forwarded as it is, so a broken source still names its row.
The group list may be empty, and the wrapper then copies the phonetic into the respelling unchanged.
The engine wraps whenever the pack declares groups, whatever the user's switch says.
The engine wraps the receiver and never the cache, so a cached lookup replays through this wrapper too.

## `public LReceiverRespelling(LReceiver inner, IReadOnlyList<LRespelling> groups)`

Both arguments are required, since a wrapper with nothing to forward to is a caller mistake.
