# LEngineRegister.cs

## `public sealed partial class LEngine`

The engine's Register seam, which mirrors the Situation one on a card.
A Register is shared data, so a card holds a reference rather than the wording.
The seam resolves what a card holds into stored rows before any reference is written.

A language's shipped Registers are written on the way in rather than at workspace creation.
So a language pack added after a workspace was made still offers its Registers.

## `public IReadOnlyList<LRegister> LEngineRegisterRead(long ownerId, LOwner owner)`

Reads the Registers a Meaning or Collocation is marked with, in that card's order.

## `public IReadOnlyList<LRegister> LEngineRegisterFind(string query, string language)`

The shelf a card offers while a Register is being typed.
It holds the rows the named language ships and every row the user has written, whatever language they came from.
A written Register crosses languages because the user wrote it for their own use.
A shipped one does not, because its wording belongs to its pack.
An empty query offers the whole shelf.

## `public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(string query, LCatalogOrder order)`

The shelf the tenor panel browses: every Register, counted and ordered.
Unlike the card's shelf, this one holds every pack's rows at once, because a workspace holds Entries of any language.
A row is answered by its name or by the language of the pack that ships it.
So a shelf of several packs can be narrowed.

## `public void LEngineRegisterChange(long registerId, string renamed)`

Renames a written Register, which every card marked with it then shows.
A row a language pack ships is left as it stands, because the pack owns the wording.
The announcement is raised whether or not the row moved, so a shown panel always re-reads.

## `public void LEngineRegisterDelete(long registerId)`

Deletes a written Register and every mark on it, which is what the panel confirms before calling.
A row a language pack ships is left whole, marks included.

## `private void LEngineRegisterCreate(string language)`

Writes the rows the named language pack ships, skipping every id already stored.

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
A positive id nothing is stored under is refused, because the card would otherwise be bound to a row the user never chose.
A row carrying a negative id is looked up by its wording first, so a name the shelf already holds is shared rather than doubled.
Only then is a new written Register created, recorded in the map under that id.

## `private LRegister? LEngineRegisterResolve(string name, string language)`

The stored Register whose name reads as `name`, or `null` when none does.
Both sides are folded the way `LCatalog.LCatalogTextNormalize` folds, so case and edge spaces do not make a second row.
The shelf offered is the one `language` sees: its own pack rows and every written row.
The engine holds this rule so the form, a commit and any other client all get one row for one wording.
