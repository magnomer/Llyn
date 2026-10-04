# LDraftClerkChip.cs
Hash: `09ac2d734490d355`

## `public sealed class LDraftClerkChip`

The tag and translation chips of a card.
Each is a link to a shared row.
A card adds a new tag by its text or picks a stored one by id.
A tag's text is edited by its id and reaches every card holding it.
A translation is only ever picked, because an entry is never written from inside another entry's card.
Situation chips live in `LSituationChip` and Register chips in `LRegisterChip`, each over its own shelf.

## `public LDraftClerkChip(LTagVault tags, LIdentity identity)`

Holds the shelf a tag is looked up on and the issuer that names a new one.

## `public LEntryDraft LTagAdd(LEntryDraft content, LRequestTagAddition request)`

A new tag with the typed text trimmed and a minted id, at the place asked for.
Blank text, or text the card already holds, leaves the draft unchanged.
Every driver and the markup import meet the same rule here, not in a view.

## `public LEntryDraft LTagInsert(LEntryDraft content, LRequestTagPick request)`

Copies the stored tag under its own id into the card, unless the card already holds it.
A stored tag whose text the card already holds is skipped too.
Both ask `LDraftClerkList.LDraftHeldCheck`, the one owner of the held-text rule.

## `public static LEntryDraft LTagChange(LEntryDraft content, LRequestTagText request)`

Rewrites the tag in every card holding it, and refuses when none does.
The text is trimmed, so every tag a draft holds reads ready to show.

## `public static LEntryDraft LTranslationInsert(LEntryDraft content, LRequestTranslationPick request)`

Links the entry, or the court's target draft, to the card, unless the card already links it.
Nothing is read from the store, because a target draft is not stored yet and the commit settles ids anyway.
An id of zero is refused with the target reason.
