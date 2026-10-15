# Tenure

This folder holds the relays for a tenure and what is built over it.
They cover observers, requests, quills, easels, forays and the trove they fill.

## TInterfaceEasel

The relays for an easel, built over a tenure.
An add relay passes the card the test names, and a situation test names zero.
Each relay is transparent.

## TInterfaceForay

The relays for a foray, one search a tenure started for its draft.
The two starts are relayed over the tenure.
The cancel, the save, the preview fetch and the flag load are relayed over the foray itself.
Each relay is transparent.
The save and the preview fetch pass on the foray's null for an outside failure.

## TInterfaceQuill

The relays for the per-subject quills, each built over a tenure.
The gloss relays name card zero and sentence zero, as the held Example does.
The chip relays build the chip edits over a tenure and a draft port a test may fake.
A second chip relay builds the chip over an engine's own draft outlet.
The reference find relay asks the chip for a source offer by text at place zero.
The situation relays build a situation quill over a tenure and set its title, kind and description.
Each relay is transparent.

## TInterfaceRequest

Builds the request records a test sends, so no test constructs a logic record itself.
Each factory returns the base type, because the engine takes the base and switches on the kind.
A factory name holds at most three components after the T prefix, counting the closing `Create`.
A one-part record such as `LRequestNote` keeps `Request`, since it fits within the limit.
A two-part record such as `LRequestSentenceAddition` drops `Request` to stay within the limit.
The card records are the exception and keep `Request` but drop `Card` instead.
`TRequestAuthorCreate` is a second exception, dropping `Name` from `LRequestAuthorName`.
It duplicates `TAuthorNameCreate`, which follows the two-part rule for the same record.

## TInterfaceRequestApply

Sends a fixture draft to the engine as the requests a form would send.

## `TRequestContentApply`

Makes a held draft read as a fixture, one request at a time.
Returns the draft as the engine last answered it.
This is how a test written against the old whole-draft save is ported without losing its fixture.
Headword, language and note are sent only when they differ, and speeches only when the fixture has some.
Every card, pronunciation and transcription the draft holds is removed before the fixture's own are added.
The fixture's rows therefore land where the fixture puts them.
Syllables, forms, inflections, reflexes, Ancestor boxes, unit and freedom are not sent, so they stay as held.
The ids the engine mints differ from the fixture's.
So a test reads them from the answer and not from the fixture.

## `TRequestCreditApply`

Credits a stored author on a stored Source the way the source form does.
It opens a draft over the Source, picks the author at the position and commits the draft.

## `TRequestQuoteApply`

Quotes a stored Example on the entry's first Meaning the way the entry form does.
It opens a draft over the entry, adds a sentence row citing the Example and commits the draft.

## `TRequestEntryApply`

Sends one form request over a stored entry and commits it, the way the entry form saves one edit.
The request is built from the draft this opens, since the draft mints its own card ids.

## `TCardSenseAttach`

Links one meaning to one collocation of a held draft and answers the draft.
It commits nothing, so a test reads the draft's answer before any save.
The detach relay drops the link the same way.

## `TRequestCardFind`

The card carrying the id, wherever it nests, for a test that needs the row the engine just minted.
It throws `InvalidOperationException` when no card carries the id.

## Inline notes

### TInterfaceRequestApply.cs: private static LDraft TRequestCardApply(

Adds one card and sends its body, its lists and its children as requests, in the order a form would.
A row or chip with a positive id is picked and one without is added new.
It sends a card's meaning links after its lists and before its children, in the fixture's order.
Each linked fixture id is first mapped to the id the draft minted for that meaning.
An id with no minted match is sent as given, so a stored meaning id still links.

### TInterfaceRequestApply.cs: private static void TRequestSenseAdd(

Pairs each fixture meaning id with the id the draft minted for it, children included.
The two trees match in shape because the meanings were just added in fixture order.

### TInterfaceRequestApply.cs: private static LDraft TRequestSentenceApply(

Adds one sentence row and sends its text, its citation and its frame.
A stored example is picked first, so the text request edits the draft copy the way the form does.

## TInterfaceTenure

The relays for a tenure, the engine's hold on one draft.
The starts and the delay seam are relayed over the engine, and every step over the tenure itself.
The linked starts of a fresh occurrence, quotation, footnote, membership and cohort are relayed beside the plain start.
The change check and the storable verdict are relayed beside the ready check.
The kept read goes through the tenure's herald, and the speech read builds a speech quill over the tenure.
The entry field edits are relayed over the tenure too.
The read, scan and set of the lexical unit and of the freedom are relayed with them.
Each relay is transparent.

## TInterfaceTenureObserver

The attach relays forward a subject, draft-identity or stored-Entry-identity filtered subscription to the tenure API.
The prepare relay exposes the tenure's prepare scope and returns its resulting draft.
The stored-id relay passes on the tenure's own answer.
The bulletin raise goes through the engine, so observer tests exercise normal event delivery.

## `TInterfaceTrove`

The relays for the per-session trove of found candidates, recordings and transcriptions.
Each relay is transparent.
The trove is internal to the engine assembly, which opens its internals to this suite.

## Stamp

Commit: `2f9a06b8eb95`
- `TInterfaceEasel.cs` `03424a4497abcd42`
- `TInterfaceForay.cs` `c27bc21fc41ef185`
- `TInterfaceQuill.cs` `b786e2b50c587e5e`
- `TInterfaceRequest.cs` `e69e88557559ec73`
- `TInterfaceRequestApply.cs` `d699bb7ec75eb444`
- `TInterfaceTenure.cs` `5d5b7864d0136076`
- `TInterfaceTenureObserver.cs` `67d3fb0c717acc3b`
- `TInterfaceTrove.cs` `235f0018d4f4689c`
