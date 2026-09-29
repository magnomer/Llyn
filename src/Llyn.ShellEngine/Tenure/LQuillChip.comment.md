# LQuillChip.cs

## `public sealed class LQuillChip`

The chip row edits of one tenure: a card's tags, situations, registers and translations.
It also finds the Sources a card sentence's citation field offers, since it holds the draft port.
It was split off `LQuill` in job39 by role, and the tag members moved here unchanged.
A typed list goes through `LDraftClerkList.LDraftListParse`, and each completed part is one request.
Its members keep the `LQuill` base, as `CDisplaySound`'s keep `CDisplay`.

## `private readonly LTenure _lQuillChipTenure;`

The tenure every request is built for and handed to.

## `private readonly LDraftPort _lQuillChipDrafts;`

The draft port that resolves a typed translation and finds every dropdown's stored rows.

## `public LQuillChip(LTenure tenure, LDraftPort drafts)`

Builds the chip edits over one tenure, which it never swaps, and the port its translations resolve through.

## `public LTagOffer LQuillTagAdd(long card, string text, int position, bool settled)`

Adds a tag for each completed part of the typed list, sent at once, and answers what the entry keeps.
`LDraftListParse` reads the list, and each tag lands after the one before it.
The clerk skips a part the card already holds.
The same answer carries the stored Tags the kept text matches, so the field paints both at once.

## `public LSituationOffer LQuillSituationAdd(long card, string text, int position, bool settled)`

Adds a situation for each completed part of the typed list, sent at once, and answers what the entry keeps.
The clerk picks a stored Situation reading the same way, and skips a title the card already shows.
The same answer carries the stored Situations the kept text matches, so the field paints both at once.

## `public void LQuillSituationInsert(long card, long situation, int position)`

Links a stored situation to a card, sent at once.

## `public void LQuillSituationRemove(long card, long situation)`

Unlinks one situation from a card, sent at once.

## `public LRegisterOffer LQuillRegisterAdd(long card, string text, int position, bool settled)`

Adds a register for each completed part of the typed list, sent at once, and answers what the entry keeps.
The clerk picks a stored Register reading the same way, and skips a name the card already shows.
The same answer carries the stored Registers the kept text matches, so the field paints both at once.

## `public void LQuillRegisterInsert(long card, long register, int position)`

Links a stored register to a card, sent at once.

## `public LReferenceOffer LQuillReferenceFind(long card, long sentence, string text)`

The stored Sources a card sentence's citation field offers for the typed text.
Typing a citation writes nothing, so this only finds.
A failed search offers nothing, since no one asked yet.

## `public LTranslationOffer LQuillTranslationAdd(long card, string text, int position)`

Links each completed word of the typed list that resolves to exactly one entry, sent at once.
The held draft's own stored entry is never its own translation, so the resolve leaves it out.
A word that resolves to no entry, or to several, stays in the text the answer keeps.
The answer also offers the Entries the kept word matches, with none chosen while the user types.
The order of the resolves, the links and the search is the engine's, so a driver makes one call.

## `public LTranslationOffer LQuillTranslationFind(string text)`

Offers the Entries the typed word matches, and marks the offer chosen when one Entry answers the whole word.
A blank word offers nothing.
A failed search reaches the caller, since the user asked for it and must hear why nothing came.

## `public string LQuillTranslationResolve(long card, string text, int position)`

Links the one Entry whose whole headword is the typed word, and answers what the entry keeps.
A linked word leaves the entry empty, and anything else stays as typed.

## `public void LQuillTranslationStart(long card, long entry, string headword, string language, int position)`

Links a row picked from the prospects.
An id of zero stands for the Entry the word would create, so a court is started for it first.
The court's target opens under the owner draft's own origin, and the link then names that target.

## `public void LQuillTranslationInsert(long card, long entry, int position)`

Links one entry or court target to a card, sent at once.
The clerk skips an entry the card already links, and refuses a zero id.

## `public void LQuillTranslationRemove(long card, long entry)`

Unlinks one entry from a card, then drops the court behind it if there was one.
The link leaves the draft before the court is touched, so nothing points at a dying target.
A stored entry has no court row, and nothing more happens.
A tentative one loses its court row and the draft it was holding, which no other draft can name.
A court that cannot be read or dropped is left alone, since the link is already gone.

## `private LTagOffer LQuillTagFind(long card, string kept)`

The stored Tags for the text a typed tag list keeps, found after the completed tags went onto the card.
A failed search offers nothing, since no one asked yet.

## `private LSituationOffer LQuillSituationFind(long card, string kept)`

The stored Situations for the text a typed situation list keeps, found after the completed titles went onto the card.
A failed search offers nothing, since no one asked yet.

## `private LRegisterOffer LQuillRegisterFind(long card, string kept)`

The stored Registers for the text a typed register list keeps, found after the completed names went onto the card.
A failed search offers nothing, since no one asked yet.

## `private LTranslationOffer LQuillProspectFind(string kept)`

The prospects for the text a typed list keeps, found as the user types.
A blank word or a failed search offers nothing, since no one asked yet.

## `public void LQuillTagInsert(long card, long tag, int position)`

Links a stored tag to a card, sent at once.

## `public void LQuillTagRemove(long card, long tag)`

Unlinks one tag from a card, sent at once.
