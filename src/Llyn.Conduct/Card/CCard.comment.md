# CCard.cs
Hash: `9020c54c39ef3302`

## `public sealed class CCard`

The gates of the editor's cards: what a card's fields ask while the user types into them.
The translation field finds prospects, resolves a typed word, and links or unlinks an Entry.
The situation and source fields find the stored rows a typed word matches.
The translation, situation, register and tag fields add what the user typed and pick stored rows.
The etymology field sends its narrative, its source links and its spans through it, over the editor's desk.
The maps from engine drafts to their shapes sit on `CFolio`.
It holds no state of its own.
The editor owns one and hands it to the card views beside the desk.

## `internal CCard(CDesk desk, LDraftPort drafts, LEntryPort entries, LSettingsPort settings, CEnvoy envoy)`

Takes the editor's desk, so an etymology request lands on the held draft.
A failed translation shows through `envoy`, with the ready notice `settings` reads.
Only the editor builds one, so the constructor is internal.

## `public CProspect CCardTranslationAdd(long cardId, string text, int position)`

The gate for the translation field's typed text, heard on every change.
The quill links each completed word that resolves to one entry and answers what the field keeps.
The same answer carries the dropdown of Entries the kept word matches, so the field paints both at once.
The dropdown comes ready below: the edited Entry left out, and the fresh Entry languages listed.
A failed resolve shows `Input.TranslationFailed`, keeps the text as typed and offers nothing.

## `public CProspect CCardTranslationResolve(long cardId, string text, int position, bool offered)`

The gate for the word standing in the translation entry, on enter or when focus leaves the field.
`offered` is the raw fact that the user pressed enter, which asks to see the choices.
Enter answers the dropdown, with one whole-headword match chosen already, so a single keystroke confirms it.
Leaving links one whole-headword match without asking, and answers what the entry keeps.
That choice between showing and linking is the interaction this gate holds.
A failed search or link shows `Input.TranslationFailed` and leaves the word in the entry.

## `public void CCardTranslationInsert(long cardId, long entryId, string headword, string language, int position)`

The gate for a row picked from the dropdown, linked to the card.
A stored Entry is linked at once, and a fresh row's word and language start a court first.
A failed start shows `Input.TranslationFailed`.

## `public void CCardTranslationRemove(long cardId, long entryId)`

The gate for a link the user erases, by its close button or a key at the entry's edge.
The quill drops the court behind a tentative link in the same call.
A failure to drop that court or its draft shows the `Input.TranslationFailed` notice, as an insert does.

## `public CProspect CCardMentionRead(string word)`

The read the editor's mention picker opens with, for a word selected or typed as the user gave it.
It searches in the editor draft's own language, through `CMention.LMentionProspectRead`.

## `internal static CProspect LCardProspectRead(LTranslationOffer offer)`

A plain map of the engine's offer, with no rule, and its rows through `CPanel.CPanelRowRead`.
The mention picker's helper in `CMention` reuses it, since both dropdowns answer the same offer.

## `public CProffer CCardReferenceFind(long cardId, long sentenceId, string text)`

The gate for text typed into a card sentence's citation field, as the user wrote it.
It answers the dropdown ready to show, and writes nothing.
The trim, the cited-byline check, the split and the cap are the reference clerk's.
With no chip quill the answer offers nothing.

## `internal static CProffer LCardProfferRead(LReferenceOffer offer)`

The plain map from the engine's citation offer to the dropdown record, carrying each row's ready count.
`CAnthology` maps the corpus drawer's offer through it too, so one citation offer has one map.

## `public IReadOnlyList<CCatalogReference> CCardReferenceRead()`

Every stored Source in author order, as the sentence menu offers them.
A failed read shows the load failure through the envoy and answers no Sources.

## `internal static CRegister LCardRegisterRead(LRegister register)`

The Conduct copy of a Register the engine handed over, its id and its shown name.
The name goes through `CFolio.CFolioStateRead`, the one map of a written value.
It is the one owner of that map, which the tenor rows read.

## `internal static CTag LCardTagRead(LTag tag)`

The Conduct copy of a Tag the engine handed over, its id and its text.
It is the one owner of that map, which the taxonomy rows read.

## `public CSlate CCardTagAdd(long cardId, string text, int position, bool settled)`

The gate for a tag typed into a card, as the user wrote it, and answers what the entry keeps.
The field hands every change unsettled, and Enter or leaving the field hands the text settled.
The same answer carries the dropdown of stored Tags the kept text matches, so the field paints both at once.
It holds no rule of its own.
The list parse and the clerk split, trim and skip blank or held text.
The tag clerk trims, filters, limits and splits the offered rows, and a failed search offers nothing.

## `private static CSlate LCardSlateRead(LTagOffer offer)`

A plain map of the engine's tag offer, with no rule.

## `public CProffer CCardSituationAdd(long cardId, string text, int position, bool settled)`

The gate for a situation typed into a card, shaped as the tag gate is.
It answers the kept text and the dropdown of stored Situations that text matches, most used first.
A card with no chip quill answers the text and no dropdown.

## `private static CProffer LCardProfferRead(LSituationOffer offer)`

The plain map from the engine's situation offer to the dropdown record, carrying each row's ready count.

## `public void CCardSituationInsert(long cardId, long situationId, int position)`

The gate for a stored situation picked from the suggestions.

## `public void CCardSituationRemove(long cardId, long situationId)`

The gate for a situation chip erased from a card.

## `public CProffer CCardRegisterAdd(long cardId, string text, int position, bool settled)`

The gate for a register typed into a card, shaped as the tag gate is.
The same answer carries the dropdown of stored Registers the kept text matches, so the field paints both at once.
The register clerk trims, filters, limits and splits the offered rows, and a failed search offers nothing.
The draft's own language seeds the shelf, so the driver passes none.

## `private static CProffer LCardProfferRead(LRegisterOffer offer)`

A plain map of the engine's register offer, with no rule.

## `public void CCardRegisterInsert(long cardId, long registerId, int position)`

The gate for a stored register picked from the suggestions.

## `public void CCardRegisterRemove(long cardId, long registerId)`

The gate for a register chip erased from a card.

## `public void CCardTagInsert(long cardId, long tagId, int position)`

The gate for a stored tag picked from the suggestions.

## `public void CCardTagRemove(long cardId, long tagId)`

The gate for a tag chip erased from a card.

## `public void CCardCitationSet(long cardId, long sentenceId, string title)`

Points the sentence's citation at the Reference the engine resolves the typed title to.
The engine reads the Reference cited now off the held draft, so an unchanged title keeps it.
One chip quill call resolves the title and cites it, since the order of those steps is a data rule.
A failure shows the create failure through the envoy and leaves the citation as it was.

## `public void CCardEtymologySet(string text)`

Defers the narrative like every typed field, so one keystroke is not one request.

## `public IReadOnlyList<CMentionLabel> CCardEtymologyRead()`

The chips of the held draft's etymology, ready to paint.
An empty desk answers no chips.
Like the old line read, it adds no failure notice of its own.

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
