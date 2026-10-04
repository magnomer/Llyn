# LQuillCard.cs
Hash: `f149000d0f9be6b3`

## `public sealed class LQuillCard`

The card list edits of one tenure: adding, removing and moving whole cards, each building one request.
It sits beside `LQuill` by role, since the quill is at its member limit.
The rules live in `LDraftClerkCard`, and this type only reads the draft and sends.
Its members keep the `LQuill` base, as `LQuillChip`'s do.

## `private readonly LTenure _lQuillCardTenure;`

The tenure every request is built for and handed to.

## `public LQuillCard(LTenure tenure)`

Builds the edits over one tenure, which they never swap.

## `public void LQuillCardAdd(LCardKind kind)`

Adds an empty card of `kind` at the place the card clerk gives a new card, sent at once.

## `public void LQuillCardRemove(long card)`

Drops one card, sent at once, unless the clerk finds it alone in its list.

## `public void LQuillCardMove(long card, int place)`

Shifts one card to a place in its own list, sent at once.
The card clerk answers whether that place is a move at all.
A card the draft lacks, a place outside its list, or the place it holds sends nothing.
So a drag hands every place its geometry finds, and only a real move becomes a request.
