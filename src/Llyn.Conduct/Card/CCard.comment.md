# CCard.cs

## `public sealed class CCard`

The gates of the editor's cards: what a card's fields ask while the user types into them.
The translation field finds prospects, resolves a typed word and opens or drops a court link.
The register, situation, tag and source fields find the stored rows a typed word matches.
The sentence menu reads the particles and dependences of a language.
The etymology field sends its narrative, its source links and its spans through it, over the editor's desk.
The maps from engine drafts to their shapes sit on `CFolio`.
It holds no state of its own.
The editor owns one and hands it to the card views beside the desk.

## `internal CCard(CDesk desk, LDraftPort drafts, LEntryPort entries)`

Takes the editor's desk, so an etymology request lands on the held draft.
Only the editor builds one, so the constructor is internal.

## `public IReadOnlyList<CCatalogReference> CCardReferenceFind()`

Every stored Source in author order, as the sentence menu offers them.
A typed word instead ranks them by usage, like the other pickers.

## `internal static CTag LCardTagRead(LTag tag)`

The Conduct copy of a Tag the engine handed over, its id and its text.
It is the one owner of that map, so the chip search and the taxonomy rows share it.

## `public void CCardTagAdd(long cardId, string text, int position)`

The gate for a tag typed into a card, as the user wrote it.
It holds no rule of its own: the clerk trims and skips blank or held text.

## `public void CCardTagInsert(long cardId, long tagId, int position)`

The gate for a stored tag picked from the suggestions.

## `public void CCardTagRemove(long cardId, long tagId)`

The gate for a tag chip erased from a card.

## `public void CCardCitationSet(long cardId, long sentenceId, string title)`

Points the sentence's citation at the Reference the engine resolves the typed title to.
The engine reads the Reference cited now off the held draft, so an unchanged title keeps it.

## `public void CCardEtymologySet(string text)`

Defers the narrative like every typed field, so one keystroke is not one request.

## `public void CCardEtymonAdd(long entryId)`

Appends a source link at the end of the row.

## `public void CCardEtymonRemove(long entryId)`

Drops one source link.

## `public void CCardMentionSave(string text, int start, int length, long entryId)`

Links a selection of the narrative to an entry, the span measured by the engine.

## `public void CCardMentionDelete(string text, int start, int length)`

Drops the span the selection lies inside, by naming entry zero over its whole length.
A selection inside no span sends nothing.

## `public bool CCardMentionCheck(string text, int start, int length)`

Whether the selection lies inside a span of the held narrative.

## `private void CCardMentionSend(LMentionDraft? span, long entryId)`

Sends one span request, or nothing when there is no span.

## `public void CCardCourtDelete(long ownerId, long targetId)`

Drops the court link between the held draft and a target, and the target's fresh draft with it.
One user action is one gate, so the translation field no longer finds the link first.

## `public static string CCardOrderRead(CSentenceOrder? order, CStateValue particle, CStateValue dependence, CStateValue text, string mark)`

Writes a sentence's frame head and its text as one line, in the language's order.
No order means the default order, with the marker written first.
The frame is dropped whole when neither field shows anything.
A legible value shows its text, and a value marked unknown shows the given mark.
Any other value shows nothing.
A caller wanting only the head passes an empty text, and one wanting only the text passes empty frame values.

## `private static string CCardLegibleRead(CStateValue state, string mark)`

The legibility rule for one frame value, shared by the three parts of the line.
