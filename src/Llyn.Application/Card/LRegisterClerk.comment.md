# LRegisterClerk.cs

## `public sealed class LRegisterClerk`

The clerk over Registers, which mirrors the Situation seam on a card.
A Register is shared data, so a card holds a reference rather than the wording.
The clerk resolves what a card holds into stored rows before any reference is written.
It runs over the register vault of one rig and raises no bulletin, which the engine keeps.

A language's shipped Registers are written on the way in rather than at workspace creation.
So a language pack added after a workspace was made still offers its Registers.

## `public LRegisterClerk(LRig rig)`

Reads the register vault out of `rig`.

## `public IReadOnlyList<LRegister> LRegisterClerkFind(string query, string language)`

The shelf a card offers while a Register is being typed.
It holds every Register, because a Register belongs to no language.
`language` only names the pack whose rows are seeded before the shelf is read, so a first card sees them.
An empty query offers the whole shelf.

## `public IReadOnlyList<LCatalogRegister> LRegisterClerkFind(string query, LCatalogOrder order)`

The shelf the tenor panel browses, every Register counted and ordered.
A row is answered by its name.

## `public LRegister LRegisterClerkCreate(string name)`

Makes a written Register reading `name` with no card marked by it yet, for the tenor panel's New.
A row already reading the same is returned rather than doubled.

## `public void LRegisterClerkPrepare(string language)`

Writes the rows the named language pack names, marking every name already stored as built in.

## `public void LRegisterClerkSync(long ownerId, IReadOnlyList<LRegisterDraft> drafts, string language, bool collocation, Dictionary<long, long> identity)`

Makes the stored marks match the card, for a card that already exists.
Marks the card no longer carries are detached, and what it carries is attached in the card's order.
A name written twice on one card is kept once, because a mark is a reference and not a row.

## `public static bool LRegisterClerkMatch(IReadOnlyList<LRegisterDraft> one, IReadOnlyList<LRegisterDraft> other)`

Whether two card fields hold the same marks in the same order.
The generated record equality compares the lists by reference, so a held draft needs this instead.

## `private static IEnumerable<LRegisterDraft> LRegisterClerkRead(IReadOnlyList<LRegisterDraft> drafts)`

The rows of the field that hold something, in the order the card gives them.

## `private long LRegisterClerkResolve(LRegisterDraft draft, Dictionary<long, long> identity)`

Turns one row of the field into the id of a stored Register.
A row that names a stored Register keeps it, and a renamed written row rewrites that row's name.
A positive id nothing is stored under is refused.
The card would otherwise be bound to a row the user never chose.
A row carrying a negative id is looked up by its wording first.
A name the shelf already holds is shared rather than doubled.
Only then is a new written Register created, recorded in the map under that id.
