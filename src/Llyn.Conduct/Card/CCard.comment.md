# CCard.cs

## `public sealed class CCard`

The gates of the editor's cards: what a card's fields ask while the user types into them.
The translation field finds prospects, resolves a typed word and opens or drops a court link.
The register, situation, tag and source fields find the stored rows a typed word matches.
The translation, situation, register and tag fields add what the user typed and pick stored rows.
The sentence menu reads the particles and dependences of a language.
The etymology field sends its narrative, its source links and its spans through it, over the editor's desk.
The maps from engine drafts to their shapes sit on `CFolio`.
It holds no state of its own.
The editor owns one and hands it to the card views beside the desk.

## `internal CCard(CDesk desk, LDraftPort drafts, LEntryPort entries, LSettingsPort settings, CEnvoy envoy)`

Takes the editor's desk, so an etymology request lands on the held draft.
A failed translation shows through `envoy`, with the ready notice `settings` reads.
Only the editor builds one, so the constructor is internal.

## `public string CCardTranslationAdd(long cardId, string text, int position)`

The gate for the translation field's typed text, heard on every change.
The quill links each completed word that resolves to one entry and answers what the field keeps.
A word that resolves to nothing stays in the answer, which is the gate's verdict for the field.
A failed resolve shows `Input.TranslationFailed` and keeps the text as typed.

## `public void CCardTranslationInsert(long cardId, long entryId, int position)`

The gate for an entry picked from the prospects, or a court target just started, linked to the card.

## `public IReadOnlyList<CRegister> CCardRegisterFind(long cardId, string word, string language)`

The stored Registers the card's field offers for a typed word, leaving out those the card already links.

## `public IReadOnlyList<CCatalogSituation> CCardSituationFind(long cardId, string word)`

The stored Situations the card's field offers for a typed word, most used first, leaving out those the card links.

## `public IReadOnlyList<CTag> CCardTagFind(long cardId, string word)`

The stored Tags the card's field offers for a typed word, most used first, leaving out those the card shows.

## `public IReadOnlyList<CCatalogReference> CCardReferenceFind()`

Every stored Source in author order, as the sentence menu offers them.
A typed word instead ranks them by usage, like the other pickers.

## `internal static CRegister LCardRegisterRead(LRegister register)`

The Conduct copy of a Register the engine handed over, its id and its shown name.
The name goes through `CFolio.CFolioStateRead`, the one map of a written value.
It is the one owner of that map, so the chip search and the tenor rows share it.

## `internal static CTag LCardTagRead(LTag tag)`

The Conduct copy of a Tag the engine handed over, its id and its text.
It is the one owner of that map, so the chip search and the taxonomy rows share it.

## `public string CCardTagAdd(long cardId, string text, int position, bool settled)`

The gate for a tag typed into a card, as the user wrote it, and answers what the entry keeps.
The field hands every change unsettled, and Enter or leaving the field hands the text settled.
It holds no rule of its own.
The list parse and the clerk split, trim and skip blank or held text.

## `public string CCardSituationAdd(long cardId, string text, int position, bool settled)`

The gate for a situation typed into a card, shaped as the tag gate is.

## `public void CCardSituationInsert(long cardId, long situationId, int position)`

The gate for a stored situation picked from the suggestions.

## `public void CCardSituationRemove(long cardId, long situationId)`

The gate for a situation chip erased from a card.

## `public string CCardRegisterAdd(long cardId, string text, int position, bool settled)`

The gate for a register typed into a card, shaped as the tag gate is.

## `public void CCardRegisterInsert(long cardId, long registerId, int position)`

The gate for a stored register picked from the suggestions.

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
