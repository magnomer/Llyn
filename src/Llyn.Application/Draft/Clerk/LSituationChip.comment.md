# LSituationChip.cs
Hash: `b7017af410e8ef14`

## `public sealed class LSituationChip`

The Situation chips of a card, each a link to a shared Situation row.
A card adds a new row by its title or picks a stored one by id.
A field of a shared Situation is edited by its id and reaches every card holding it.
Its change also reaches the situation panel's own draft, which holds no cards.

## `public LSituationChip(LSituationVault situations, LIdentity identity)`

Holds the shelf a Situation is looked up on and the issuer that names a new one.

## `public static LDraft? LSituationChipApply(LDraft draft, LRequest request)`

Routes every situation field request to the change by id, and answers null for any other request.
A field request may mean the panel's situation or a chip, so it is routed by id.
The clerk then hands any other request on to the entry content.

## `public LEntryDraft? LSituationChipApply(LEntryDraft content, LRequest request)`

Routes every situation chip request on a card to its handler, and answers null for any other request.
The clerk then hands that request on to the Register chips.

## `public LEntryDraft LSituationChipAdd(LEntryDraft content, LRequestSituationAddition request)`

A situation with the typed title, at the place asked for.
The title is looked up first, and a stored Situation reading the same way is picked under its own id.
Only a title that no single stored Situation matches becomes a new situation with a minted id.
A title the card already shows is nothing to add, so a typed duplicate leaves the draft unchanged.
The chip holds this rule so the form, an import and a test all get one row for one wording.

## `public LEntryDraft LSituationChipInsert(LEntryDraft content, LRequestSituationPick request)`

Copies the stored situation under its own id into the card, unless the card already holds it.
An id naming no stored situation is refused.

## `public static LEntryDraft LSituationChipRemove(LEntryDraft content, LRequestSituationRemoval request)`

Drops the situation carrying the id from the card, and refuses when the card holds none.

## `public static LEntryDraft LSituationChipMove(LEntryDraft content, LRequestSituationShift request)`

Moves the situation carrying the id to the place asked for, and refuses when the card holds none.

## `public static LDraft LSituationChipChange(LDraft draft, long id, Func<LSituation, LSituation> change)`

Applies one change to the situation named, wherever the draft holds it.
The situation panel's own situation is looked at first, since a panel draft holds no cards.
Otherwise every card is offered the change, and a draft holding no such situation is refused.
The change is written against the stored shape and mapped onto the chip shape, so one lambda serves both.

## `public static LSituation? LSituationChipResolve(LSituationVault situations, LStateValue title)`

The one stored Situation whose folded title is the folded title given, or null.
Two stored Situations reading the same way answer null too, since neither is the one meant.
`LCardClerkField.LSituationResolve` asks the same question when it settles a chip, so the rule is written here once.

## `private static LSituationDraft LSituationChipRead(LSituation stored)`

The chip shape of a stored situation.

## `private static LSituation LSituationChipRead(LSituationDraft draft)`

The stored shape of a chip, for a change written against the stored shape.

## `private static LEntryDraft LSituationChipApply(LEntryDraft content, long cardId, Func<IReadOnlyList<LSituationDraft>, IReadOnlyList<LSituationDraft>> change)`

Rewrites the situation list of the card named through the list routine given.
A card the draft does not hold is refused by the card change.
