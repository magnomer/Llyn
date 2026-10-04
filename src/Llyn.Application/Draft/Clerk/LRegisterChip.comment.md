# LRegisterChip.cs
Hash: `22ec952482a79fe4`

## `public sealed class LRegisterChip`

The Register chips of a card, each a link to a shared Register row.
A card adds a new row by its name or picks a stored one by id.
A rename is made by the Register's id and reaches every card holding it.

## `public LRegisterChip(LRegisterVault registers, LIdentity identity)`

Holds the shelf a Register is looked up on and the issuer that names a new one.

## `public LEntryDraft LRegisterChipAdd(LEntryDraft content, LRequestRegisterAddition request)`

A register with the typed name, at the place asked for.
The name is looked up on the whole shelf.
A stored Register reading the same way is picked under its own id.
Only a name nothing matches becomes a new register with a minted id.
A name the card already shows is nothing to add, so a typed duplicate leaves the draft unchanged.

## `public LEntryDraft LRegisterChipInsert(LEntryDraft content, LRequestRegisterPick request)`

Copies the stored register under its own id into the card, unless the card already holds it.

## `public static LEntryDraft LRegisterChipChange(LEntryDraft content, LRequestRegisterName request)`

Renames the register in every card holding it, and refuses when none does.

## `public static LRegister? LRegisterChipResolve(LRegisterVault registers, string name)`

The stored Register whose folded name is the folded name given, or null for a blank or unknown name.
`LRegisterClerk` asks the same question when it creates a Register and when it settles a card's row.
The rule is written here once.
