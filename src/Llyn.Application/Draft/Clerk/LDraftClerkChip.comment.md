# LDraftClerkChip.cs
Hash: `b96e58b192f9a9ea`

## `public sealed class LDraftClerkChip`

The tag and translation chips of a card.
Each is a link to a shared row.
A card adds a new tag by its text or picks a stored one by id.
A tag's text is edited by its id and reaches every card holding it.
A translation is only ever picked, because an entry is never written from inside another entry's card.
Situation chips live in `LSituationChip` and Register chips in `LRegisterChip`, each over its own shelf.

## `public LDraftClerkChip(LTagVault tags, LIdentity identity)`

Holds the shelf a tag is looked up on and the issuer that names a new one.

## `public LEntryDraft? LTagApply(LEntryDraft content, LRequest request)`

Routes every tag chip request to its handler, and answers null for any other request.
The clerk then hands that request on to the translation chips.

## `public static LEntryDraft? LTranslationApply(LEntryDraft content, LRequest request)`

Routes every translation chip request to its handler, and answers null for any other request.
The clerk then hands that request on to the picture rows.

## `public LEntryDraft LTagAdd(LEntryDraft content, LRequestTagAddition request)`

A new tag with the typed text trimmed and a minted id, at the place asked for.
Blank text, or text the card already holds, leaves the draft unchanged.
Every driver meets the same rule here, not in a view.

## `public LEntryDraft LTagInsert(LEntryDraft content, LRequestTagPick request)`

Copies the stored tag under its own id into the card, unless the card already holds it.
A stored tag whose text the card already holds is skipped too.
`LDraftClerkList.LDraftListInsert` holds the id test.
`LDraftClerkList.LDraftHeldCheck` holds the text test, and is the one owner of the held-text rule.

## `public static LEntryDraft LTagRemove(LEntryDraft content, LRequestTagRemoval request)`

Drops the tag carrying the id from the card, and refuses when the card holds none.

## `public static LEntryDraft LTagMove(LEntryDraft content, LRequestTagShift request)`

Moves the tag carrying the id to the place asked for, and refuses when the card holds none.

## `public static LEntryDraft LTagChange(LEntryDraft content, LRequestTagText request)`

Rewrites the tag in every card holding it, and refuses when none does.
The text is trimmed, so every tag a draft holds reads ready to show.

## `public static LEntryDraft LTranslationInsert(LEntryDraft content, LRequestTranslationPick request)`

Links the entry, or the court's target draft, to the card, unless the card already links it.
Nothing is read from the store, because a target draft is not stored yet and the commit settles ids anyway.
An id of zero is refused with the target reason.

## `public static LEntryDraft LTranslationRemove(LEntryDraft content, LRequestTranslationRemoval request)`

Drops the link to the entry from the card, and refuses when the card holds none.

## `public static LEntryDraft LTranslationMove(LEntryDraft content, LRequestTranslationShift request)`

Moves the link to the entry to the place asked for, and refuses when the card holds none.
