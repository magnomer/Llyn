# LDraftClerkChip.cs

## `public sealed class LDraftClerkChip`

The chips of a card: situations, registers, tags and translations.
Each is a link to a shared row.
A card adds a new row by its wording or picks a stored one by id.
A field of a shared row is edited by the row's id and reaches every card holding it.
A translation is only ever picked, because an entry is never written from inside another entry's card.

## `public LDraftClerkChip(`

Holds the three shelves a chip is looked up on and the issuer that names a new one.

## `public LEntryDraft LSituationAdd(LEntryDraft content, LRequestSituationAddition request)`

A situation with the typed title, at the place asked for.
The title is looked up first, and a stored Situation reading the same way is picked under its own id.
Only a title nothing matches becomes a new situation with a minted id.
The clerk holds this rule so the form, an import and a test all get one row for one wording.

## `public LEntryDraft LSituationInsert(LEntryDraft content, LRequestSituationPick request)`

Copies the stored situation under its own id into the card, unless the card already holds it.
An id naming no stored situation is refused.

## `public static LDraft LSituationChange(LDraft draft, long id, Func<LSituation, LSituation> change)`

Applies one change to the situation named, wherever the draft holds it.
The situation panel's own situation is looked at first, since a panel draft holds no cards.
Otherwise every card is offered the change, and a draft holding no such situation is refused.
The change is written against the stored shape and mapped onto the chip shape, so one lambda serves both.

## `public static LSituation? LSituationResolve(LSituationVault situations, LStateValue title)`

The one stored Situation whose folded title is the folded title given, or null.
Two stored Situations reading the same way answer null too, since neither is the one meant.
The engine's commit asks the same question, so the rule is written here once.

## `private static LSituationDraft LSituationRead(LSituation stored)`

The chip shape of a stored situation.

## `private static LSituation LSituationRead(LSituationDraft draft)`

The stored shape of a chip, for a change written against the stored shape.

## `public LEntryDraft LRegisterAdd(LEntryDraft content, LRequestRegisterAddition request)`

A register with the typed name, at the place asked for.
The name is looked up on the whole shelf.
A stored Register reading the same way is picked under its own id.
Only a name nothing matches becomes a new register with a minted id.

## `public LEntryDraft LRegisterInsert(LEntryDraft content, LRequestRegisterPick request)`

Copies the stored register under its own id into the card, unless the card already holds it.

## `public static LEntryDraft LRegisterChange(LEntryDraft content, LRequestRegisterName request)`

Renames the register in every card holding it, and refuses when none does.

## `public static LRegister? LRegisterResolve(LRegisterVault registers, string name)`

The stored Register whose folded name is the folded name given, or null for a blank or unknown name.
The engine's commit and its register creation ask the same question, so the rule is written here once.

## `public LEntryDraft LTagAdd(LEntryDraft content, LRequestTagAddition request)`

A new tag with the typed text and a minted id, at the place asked for.

## `public LEntryDraft LTagInsert(LEntryDraft content, LRequestTagPick request)`

Copies the stored tag under its own id into the card, unless the card already holds it.

## `public static LEntryDraft LTagChange(LEntryDraft content, LRequestTagText request)`

Rewrites the tag in every card holding it, and refuses when none does.

## `public static LEntryDraft LTranslationInsert(LEntryDraft content, LRequestTranslationPick request)`

Links the entry, or the court's target draft, to the card, unless the card already links it.
Nothing is read from the store, because a target draft is not stored yet and the commit settles ids anyway.
An id of zero is refused with the target reason.
