# TInterfaceEngineLanguage.cs
Hash: `b5fce1b3a7374ccb`

## `internal static class TInterfaceEngineLanguage`

The relays for the engine's language fills over an entry.
That is the ensign load, the frequency, the inflection, the script, the reflex and the stem.
The paradigm scan and the language the paradigm resolves for an entry are relayed too.
Each fill is relayed with its read, its pending check, its start and its rebuild where it has them.
The sound start relays the reading view's start, which starts every fill of the entry at once.
The frequency also relays its gauge, the answer the entry view's chip and tooltip show.
Each relay is transparent and carries no test logic of its own.
The kindred relay takes an optional vista, so a test can pin the order the panel sets.

## `internal static async Task<IReadOnlyList<LEnsignRow>> TEngineEnsignLoad(this LEngine engine)`

The engine hands the rows to a callback, so the relay keeps them and answers them once the load ends.
The overload for one language and its varieties keeps them the same way.

## `internal static Task<IReadOnlyList<string>> TEngineEnsignLoad(this LEngine engine, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)`

Hands the test's own `store` to the engine's fill and answers the languages the fill returns.
It is the call the settings outlet forwards to, so a test reads both the stored rows and the answer.

## `internal static LFrequencyGauge? TEngineFrequencyResolve(this LEngine engine, long entryId, string once)`

Relays the gauge the entry view's chip and tooltip show, resolved over the stored rows.

## `internal static LParadigmView? TEngineInflectionRead(this LEngine engine, long entryId, bool pending, bool enabled, bool held = false)`

`pending` and `enabled` stand for the fetch check and the morphology setting the shell passes in.
`held` defaults off, as the reading view and the note ask.
A test sets them directly, so it reaches every cell tip the view shows without a live fetch.

## `internal static void TEngineStemSpread(this LEngine engine, long? id, string character, bool opened)`

Relays a series member's fold toggle, the call the xiesheng gate makes.
