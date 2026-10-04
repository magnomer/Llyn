# TInterfaceEngineLanguage.cs
Hash: `b6e1e70235375669`

## `internal static class TInterfaceEngineLanguage`

The relays for the engine's language fills over an entry.
That is the ensign load, the frequency, the inflection, the script, the reflex and the stem.
Each fill is relayed with its read, its pending check, its start and its rebuild where it has them.
Each relay is transparent and carries no test logic of its own.

## `internal static async Task<IReadOnlyList<LEnsignRow>> TEngineEnsignLoad(this LEngine engine)`

The engine hands the rows to a callback, so the relay keeps them and answers them once the load ends.
The overload for one language and its varieties keeps them the same way.

## `internal static Task<IReadOnlyList<string>> TEngineEnsignLoad(`

Hands the test's own `store` to the engine's fill and answers the languages the fill returns.
It is the call the settings outlet forwards to, so a test reads both the stored rows and the answer.
