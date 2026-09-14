# LEngineRequestChip.cs

## `public sealed partial class LEngine`

The chips of a card: situations, registers, tags and translations.
Each is a link to a shared row.
A card adds a new row by its wording or picks a stored one by id.
A field of a shared row is edited by the row's id and reaches every card holding it.
A translation is only ever picked, because an entry is never written from inside another entry's card.

## `private LEntryDraft LEngineSituationAdd(LEntryDraft content, LRequestSituationAddition request)`

A situation with the typed title, at the place asked for.
The title is looked up first, and a stored Situation reading the same way is picked under its own id.
Only a title nothing matches becomes a new situation with a minted id.
The engine holds this rule so the form, an import and a test all get one row for one wording.

## `private LEntryDraft LEngineSituationInsert(LEntryDraft content, LRequestSituationPick request)`

Copies the stored situation under its own id into the card, unless the card already holds it.
An id naming no stored situation is refused.

## `private static LDraft LEngineSituationChange(LDraft draft, long id, Func<LSituation, LSituation> change)`

Applies one change to the situation named, wherever the draft holds it.
The situation panel's own situation is looked at first, since a panel draft holds no cards.
Otherwise every card is offered the change, and a draft holding no such situation is refused.
The change is written against the stored shape and mapped onto the chip shape, so one lambda serves both.

## `private static LSituationDraft LEngineSituationRead(LSituation stored)`

The chip shape of a stored situation.

## `private static LSituation LEngineSituationRead(LSituationDraft draft)`

The stored shape of a chip, for a change written against the stored shape.

## `private LEntryDraft LEngineRegisterAdd(LEntryDraft content, LRequestRegisterAddition request)`

A register with the typed name, at the place asked for.
The name is looked up on the whole shelf.
A stored Register reading the same way is picked under its own id.
Only a name nothing matches becomes a new register with a minted id.

## `private LEntryDraft LEngineRegisterInsert(LEntryDraft content, LRequestRegisterPick request)`

Copies the stored register under its own id into the card, unless the card already holds it.

## `private static LEntryDraft LEngineRegisterChange(LEntryDraft content, LRequestRegisterName request)`

Renames the register in every card holding it, and refuses when none does.

## `private LEntryDraft LEngineTagAdd(LEntryDraft content, LRequestTagAddition request)`

A new tag with the typed text and a minted id, at the place asked for.

## `private LEntryDraft LEngineTagInsert(LEntryDraft content, LRequestTagPick request)`

Copies the stored tag under its own id into the card, unless the card already holds it.

## `private static LEntryDraft LEngineTagChange(LEntryDraft content, LRequestTagText request)`

Rewrites the tag in every card holding it, and refuses when none does.

## `private static LEntryDraft LEngineTranslationInsert(LEntryDraft content, LRequestTranslationPick request)`

Links the entry, or the court's target draft, to the card, unless the card already links it.
Nothing is read from the store, because a target draft is not stored yet and the commit settles ids anyway.
An id of zero is refused with the target reason.
