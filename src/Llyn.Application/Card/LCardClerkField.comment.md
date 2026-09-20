# LCardClerkField.cs

## `public static class LCardClerkField`

The stateless helpers over the fields a card carries.
They decide which draft rows are worth writing.
They also resolve a row into the stored Situation, Image or Video it names.
The card clerk, the meaning clerk and the engine's situation part all call them.
Nothing here holds a vault, so every resolver takes the vault it writes through.

## `public static void LCardValidate(IReadOnlyList<LCardDraft> cards, bool collocation)`

A Collocation carrying a card inside it is refused before anything is written.
Only a Meaning nests, because only a sense names a parent in the store.
Saving it with the children dropped would lose data the caller believes it handed over.

## `public static IEnumerable<LCardDraft> LCardRead(IReadOnlyList<LCardDraft> cards)`

The cards worth a row.
A card with every field blank is neither a Meaning nor a Collocation.
So it is skipped rather than written.
A card carrying nothing but a Translation is a card, because a link is something the user chose.
The form always hands over at least one card of each kind.
It seeds one of each and refuses to remove a list's last card.
So this is where an entry with nothing typed stops becoming rows.
Positions come from the stored sibling count.
So skipping a card in the middle still leaves 0, 1, 2 over the cards that remain.

## `public static IEnumerable<LSentenceDraft> LSentenceRead(IReadOnlyList<LSentenceDraft> drafts)`

The rows worth writing, in the order the card holds them.
A row stating a frame and no sentence is kept, because the frame is the card's own.
The positions stay a gapless 0, 1, 2 over what is actually stored.

## `public static IEnumerable<LSituationDraft> LSituationRead(IReadOnlyList<LSituationDraft> drafts)`

The Situation chips of one card that carry a title, in card order.

## `public static long LSituationResolve(LSituationVault situations, LSituationDraft draft, Dictionary<long, long> identity)`

The stored Situation a chip's positive id names, updated to what the chip now says.
A positive id nothing is stored under is refused rather than rebound.
A chip carrying a negative id is looked up by its title first.
A wording the workspace already holds is shared rather than doubled.
Only then is a fresh Situation made, recorded in the map under the negative id it replaces.

## `public static IEnumerable<LVideoDraft> LVideoRead(IReadOnlyList<LVideoDraft> rows)`

The Videos of one card that are worth a row, on the same terms as an Image.

## `public static IEnumerable<LImageDraft> LImageRead(IReadOnlyList<LImageDraft> rows)`

The Images of one card that are worth a row.
Rows naming no picture are dropped here rather than written.
So a list the shell built out of a control cannot leave an empty row behind.
The positions stay a gapless 0, 1, 2 over what is actually stored.

## `public static long LImageResolve(LImageVault images, LImageDraft draft, Dictionary<long, long> identity)`

The stored Image a row's id names, updated to the location the row now says.
A positive id nothing is stored under is refused rather than rebound to a fresh row.
A row carrying a negative id gets a fresh Image instead.
The fresh row is recorded in the map under the negative id it replaces.
Two cards naming one id therefore reference one row, which is what sharing a picture means.

## `public static long LVideoResolve(LVideoVault videos, LVideoDraft draft, Dictionary<long, long> identity)`

The same rule for a Video, whose location and span are both updated when either changed.

## `public static void LCardFieldSync<LCardRow, LCardWritten>(IEnumerable<LCardWritten> written, IReadOnlyList<LCardRow> attached, Func<LCardRow, long> identify, Func<LCardWritten, long> resolve, Action<long> detach, Action<long, int> attach)`

One card field reconciled against the rows it already references.
Situation, Register, Image and Video differ only in two things.
Those are which resolver turns a value into a row id, and which pair of methods attaches and detaches it.
So they are handed in and the reconciliation itself is written once.

A value names its row by id and nothing else.
A positive id keeps that row, and a negative id creates one.
A card naming one id twice keeps one association.
Attaching a row the card already references moves it.
The association goes in at the end and the set is renumbered around the requested position.
So draft order becomes stored order.
