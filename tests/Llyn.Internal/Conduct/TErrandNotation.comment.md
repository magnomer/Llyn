# TErrandNotation.cs

## `public sealed class TErrandNotation`

Covers the notation popup's gates end to end on a real editor: start, flag load and the reading pick.
A padded headword starts a search whose every step passes through the desk's marshal and raises the ready state.
The marshal queues each step and the test runs them in turn, so the gated source fixes their order.
The draft is persisted before the marshal attaches, so exactly the search's three steps queue.
A step of a search replaced by a later start lands after it and lists nothing.
A blank headword starts nothing, answers the empty notice and still stops a running recording search.
The flag load stores the variety flags for an IPA search only, and answers the state to repaint.
A found reading lists its text and mark as the search's settings answer them.
It shows a flag only when it names a variety.
A schemed search lists the phonetic bare, whatever the respelling switch shows.
A source without a reading says missing or broken, and an empty answer after a reading keeps it.
Rows keep the pack's order once each, and the end of a schemed search reads the transcription notice.
A pick writes the primary IPA and its variety at once, before any persist.
An accent row takes the pick, and a row removed since the start takes nothing.
A schemed search writes the transcription text at once and names no variety.
No search, or a desk still filling, writes nothing.

## `private const string TErrandNotationPack`

A one-source pack whose IPA source answers one British reading.

## `private static async Task<Action> TErrandStepRead(Channel<Action> marshalled)`

Takes the next step the desk's marshal queued, failing after five seconds so a lost step cannot hang the run.

## `private static string TErrandNotationRead(CNotationRoll roll)`

Reads a notation state as each row's source and reading texts, for comparing the raised states in order.

## `private static CEditor TErrandNotationPrepare(LEngine engine, string language, string headword)`

Opens a new draft in the pack's language with the headword typed, as the input tab holds it.
