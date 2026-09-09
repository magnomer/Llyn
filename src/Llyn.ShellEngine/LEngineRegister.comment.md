# LEngineRegister.cs

## `public sealed partial class LEngine`

The engine's Register seam, which mirrors the Situation one on a card.
A Register is shared data, so a card holds a reference rather than the wording.
The seam resolves what a card holds into stored rows before any reference is written.

A language's shipped Registers are written on the way in rather than at workspace creation.
So a language pack added after a workspace was made still offers its Registers.

## `public IReadOnlyList<LRegister> LEngineRegisterRead(string ownerId, LOwner owner)`

Reads the Registers a Meaning or Collocation is marked with, in that card's order.

## `public IReadOnlyList<LRegister> LEngineRegisterFind(string query, string language)`

The shelf a card offers while a Register is being typed.
It holds the rows the named language ships and every row the user has written, whatever language they came from.
A written Register crosses languages because the user wrote it for their own use.
A shipped one does not, because its wording belongs to its pack.
An empty query offers the whole shelf.

## `private void LEngineRegisterCreate(string language)`

Writes the rows the named language pack ships, skipping every id already stored.

## `private void LEngineRegisterSync(string ownerId, IReadOnlyList<LRegisterDraft> drafts, string language, bool collocation)`

Makes the stored marks match the card, for a card that already exists.
Marks the card no longer carries are detached, and what it carries is attached in the card's order.
A name written twice on one card is kept once, because a mark is a reference and not a row.

## `private void LEngineRegisterAttach(string ownerId, IReadOnlyList<LRegisterDraft> drafts, string language, bool collocation)`

Writes the marks of a card being created, where nothing stands to be detached.

## `private static bool LEngineRegisterMatch(IReadOnlyList<LRegisterDraft> one, IReadOnlyList<LRegisterDraft> other)`

Whether two card fields hold the same marks in the same order.
The generated record equality compares the lists by reference, so a held draft needs this instead.

## `private static bool LEngineRegisterCheck(IReadOnlyList<LRegisterDraft> drafts)`

Whether any row of the field holds something, so an otherwise empty card is not dropped.

## `private static IEnumerable<LRegisterDraft> LEngineRegisterRead(IReadOnlyList<LRegisterDraft> drafts)`

The rows of the field that hold something, in the order the card gives them.

## `private static string LEngineRegisterResolve(LRegisterArchive registers, LRegisterDraft draft)`

Turns one row of the field into the id of a stored Register.
A row that names a stored Register keeps it, and a renamed written row rewrites that row's name.
A row naming nothing stored is matched against the shelf by wording, so typing a name twice never doubles the shelf.
Only a wording nothing on the shelf carries creates a new written Register.
