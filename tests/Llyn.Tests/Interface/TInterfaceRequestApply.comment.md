# TInterfaceRequestApply.cs

## `internal static partial class TInterface`

Sends a whole fixture draft to the engine as the requests a form would send.

## `internal static LDraft TRequestContentApply(this LEngine engine, long draftId, LEntryDraft content)`

Makes a held draft read as a fixture, one request at a time.
Returns the draft as the engine last answered it.
This is how a test written against the old whole-draft save is ported without losing its fixture.
Every card, pronunciation and transcription the draft holds is removed first.
The fixture's rows therefore land where the fixture puts them.
The ids the engine mints differ from the fixture's.
So a test reads them from the answer and not from the fixture.

## `internal static LCardDraft TRequestCardFind(LEntryDraft content, long cardId)`

The card carrying the id, wherever it nests, for a test that needs the row the engine just minted.

## Inline notes

### `private static LDraft TRequestCardApply(`

Adds one card and sends its body, its lists and its children as requests, in the order a form would.
A row or chip with a positive id is picked and one without is added new.

### `private static LDraft TRequestSentenceApply(`

Adds one sentence row and sends its text, its citation and its frame.
A stored example is picked first, so the text request edits the draft copy the way the form does.
