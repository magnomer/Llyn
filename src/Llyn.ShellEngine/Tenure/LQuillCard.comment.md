# LQuillCard.cs

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
The clerk clamps the place, so a dragged index needs no check here.

## `public void LQuillCardMove(long card, string ordinal)`

Moves one card to the place a typed number names, as the card clerk reads it.
Text naming no new place sends nothing.
