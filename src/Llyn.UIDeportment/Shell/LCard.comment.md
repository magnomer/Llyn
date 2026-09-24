# LCard.cs

## `public sealed class LCard`

The deportment of the editor's cards: what a card's fields ask the engine while the user types into them.
The translation field finds prospects, resolves a typed word and opens, finds or drops a court link.
The register, situation, tag and source fields find the stored rows a typed word matches.
The sentence menu reads the frame, particles and dependences of a language.
The etymology field sends its narrative, its source links and its spans through it, over the editor's desk.
It holds no state, since the card drag and the chip picks stay in the veneer until their own plan.
The editor deportment owns one and hands it to the card views beside the desk.

## `public LCard(LDesk desk, LDraftPort drafts, LEntryPort entries, LPhonologyPort phonology)`

Takes the editor's desk, so an etymology request lands on the held draft.

## `public void LCardCitationSet(long cardId, long sentenceId, string title)`

Points the sentence's citation at the Reference the engine resolves the typed title to.
The engine reads the Reference cited now off the held draft, so an unchanged title keeps it.

## `public void LCardEtymologySet(string text)`

Defers the narrative like every typed field, so one keystroke is not one request.

## `public void LCardEtymonAdd(long entryId)`

Appends a source link at the end of the row.

## `public void LCardEtymonRemove(long entryId)`

Drops one source link.

## `public void LCardMentionSave(string text, int start, int length, long entryId)`

Links a selection of the narrative to an entry, the span measured by the engine.

## `public void LCardMentionDelete(string text, int start, int length)`

Drops the span the selection lies inside, by naming no entry over its whole length.
A selection inside no span sends nothing.

## `public bool LCardMentionCheck(string text, int start, int length)`

Whether the selection lies inside a span of the held narrative.

## `private LEtymologyDraft LCardEtymologyRead()`

The held draft's etymology, or an empty one while nothing is held.

## `private void LCardMentionSend(LMentionDraft? span, long entryId)`

Sends one span request, or nothing when there is no span.
