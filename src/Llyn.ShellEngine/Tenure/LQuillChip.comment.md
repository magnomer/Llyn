# LQuillChip.cs

## `public sealed class LQuillChip`

The chip row edits of one tenure: a card's tags, situations, registers and translations.
It was split off `LQuill` in job39 by role, and the tag members moved here unchanged.
A typed list goes through `LDraftClerkList.LDraftListParse`, and each completed part is one request.
Its members keep the `LQuill` base, as `CDisplaySound`'s keep `CDisplay`.

## `private readonly LTenure _lQuillChipTenure;`

The tenure every request is built for and handed to.

## `private readonly LDraftPort _lQuillChipDrafts;`

The draft port a typed translation is resolved through.

## `public LQuillChip(LTenure tenure, LDraftPort drafts)`

Builds the chip edits over one tenure, which it never swaps, and the port its translations resolve through.

## `public string LQuillTagAdd(long card, string text, int position, bool settled)`

Adds a tag for each completed part of the typed list, sent at once, and answers what the entry keeps.
`LDraftListParse` reads the list, and each tag lands after the one before it.
The clerk skips a part the card already holds.

## `public string LQuillSituationAdd(long card, string text, int position, bool settled)`

Adds a situation for each completed part of the typed list, sent at once, and answers what the entry keeps.
The clerk picks a stored Situation reading the same way, and skips a title the card already shows.

## `public void LQuillSituationInsert(long card, long situation, int position)`

Links a stored situation to a card, sent at once.

## `public void LQuillSituationRemove(long card, long situation)`

Unlinks one situation from a card, sent at once.

## `public string LQuillRegisterAdd(long card, string text, int position, bool settled)`

Adds a register for each completed part of the typed list, sent at once, and answers what the entry keeps.
The clerk picks a stored Register reading the same way, and skips a name the card already shows.

## `public void LQuillRegisterInsert(long card, long register, int position)`

Links a stored register to a card, sent at once.

## `public string LQuillTranslationAdd(long card, string text, int position)`

Links each completed word of the typed list that resolves to exactly one entry, sent at once.
The held draft's own stored entry is never its own translation, so the resolve leaves it out.
A word that resolves to no entry, or to several, stays in the answer for the user to fix.
The order of the resolves and the links is the engine's, so a driver makes one call.

## `public void LQuillTranslationInsert(long card, long entry, int position)`

Links one entry or court target to a card, sent at once.
The clerk skips an entry the card already links, and refuses a zero id.

## `public void LQuillTagInsert(long card, long tag, int position)`

Links a stored tag to a card, sent at once.

## `public void LQuillTagRemove(long card, long tag)`

Unlinks one tag from a card, sent at once.
