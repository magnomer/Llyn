# LEngineRegister.cs

## `public sealed partial class LEngine`

The engine's Register seam, which mirrors the Situation one on a card.
A Register is shared data, so a card holds a reference rather than the wording.
The seam resolves what a card holds into stored rows before any reference is written.

A language's shipped Registers are written on the way in rather than at workspace creation.
So a language pack added after a workspace was made still offers its Registers.

## `internal IReadOnlyList<LRegister> LEngineRegisterRead(long ownerId, LOwner owner)`

Reads the Registers a Meaning or Collocation is marked with, in that card's order.

## `public IReadOnlyList<LRegister> LEngineRegisterFind(string query, string language)`

The shelf a card offers while a Register is being typed.
It holds every Register, because a Register belongs to no language.
`language` only names the pack whose rows are seeded before the shelf is read, so a first card sees them.
An empty query offers the whole shelf.

## `public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(string query, LCatalogOrder order)`

The shelf the tenor panel browses: every Register, counted and ordered.
A row is answered by its name.

## `public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista)`

The shelf the tenor panel's vista lists, with the query and order read off the vista.
The vista's filter hides languages from the entries of the chosen Register, not Registers, so it is not applied here.

## `public LRegister LEngineRegisterCreate(string name)`

Makes a written Register reading `name` with no card marked by it yet, for the tenor panel's New.
A row already reading the same is returned rather than doubled.
The announcement is raised either way, so the panel lists and selects the row.

## `internal void LEngineRegisterChange(long registerId, string renamed)`

Renames a written Register, which every card marked with it then shows.
A row a language pack ships is left as it stands, because the pack owns the wording.
The announcement is raised whether or not the row moved, so a shown panel always re-reads.

## `internal void LEngineRegisterDelete(long registerId)`

Deletes a written Register and every mark on it, which is what the panel confirms before calling.
A row a language pack ships is left whole, marks included.

## `private void LEngineRegisterPrepare(string language)`

Writes the rows the named language pack names, marking every name already stored as built in.

## `private void LEngineRegisterSync(long ownerId, IReadOnlyList<LRegisterDraft> drafts, string language, bool collocation)`

Makes the stored marks match the card, for a card that already exists.
Marks the card no longer carries are detached, and what it carries is attached in the card's order.
A name written twice on one card is kept once, because a mark is a reference and not a row.

## `private static bool LEngineRegisterMatch(IReadOnlyList<LRegisterDraft> one, IReadOnlyList<LRegisterDraft> other)`

Whether two card fields hold the same marks in the same order.
The generated record equality compares the lists by reference, so a held draft needs this instead.

## `private static bool LEngineRegisterCheck(IReadOnlyList<LRegisterDraft> drafts)`

Whether any row of the field holds something, so an otherwise empty card is not dropped.

## `private static IEnumerable<LRegisterDraft> LEngineRegisterRead(IReadOnlyList<LRegisterDraft> drafts)`

The rows of the field that hold something, in the order the card gives them.

## `private long LEngineRegisterResolve(`

Turns one row of the field into the id of a stored Register.
A row that names a stored Register keeps it, and a renamed written row rewrites that row's name.
A positive id nothing is stored under is refused.
The card would otherwise be bound to a row the user never chose.
A row carrying a negative id is looked up by its wording first.
A name the shelf already holds is shared rather than doubled.
Only then is a new written Register created, recorded in the map under that id.

## `private LRegister? LEngineRegisterResolve(string name)`

The stored Register whose name reads as `name`, or `null` when none does.
Both sides are folded the way `LCatalog.LCatalogTextNormalize` folds, so case and edge spaces do not make a second row.
The whole shelf is searched, because a name is one Register whichever pack or card named it.
The engine holds this rule.
The form, a commit and any other client therefore all get one row for one wording.
