# TErrandNotation.cs
Hash: `4adafd8bf93e48a8`

## `public sealed class TErrandNotation`

Covers the notation popup's start and flag load end to end on a real editor.
A padded headword starts a search whose every step passes through the desk's marshal and raises the ready state.
The marshal queues each step and the test runs them in turn, so the gated source fixes their order.
The draft is read through the desk before the marshal attaches, so exactly the search's three steps queue.
A step of a search replaced by a later start lands after it and lists nothing.
A blank headword starts nothing, answers the empty notice and still stops a running recording search.
The flag load stores the variety flags for an IPA search only, and answers the state to repaint.
The reading pick lives in `TErrandNotationReading`, and the listed rows live in `TErrandNotationRoll`.

## `internal const string TErrandNotationPack`

A one-source pack whose IPA source answers one British reading.

## `internal static async Task<Action> TErrandStepRead(Channel<Action> marshalled)`

Takes the next step the desk's marshal queued, failing after five seconds so a lost step cannot hang the run.

## `internal static string TErrandNotationRead(CNotationRoll roll)`

Reads a notation state as each row's source and reading texts, for comparing the raised states in order.

## `internal static CEditor TErrandNotationPrepare(LEngine engine, string language, string headword)`

Opens a new draft in the pack's language with the headword typed, as the input tab holds it.
`TErrandNotationRoll` and most of `TErrandNotationReading` build their editor through it.
