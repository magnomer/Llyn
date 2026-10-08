# LQuillCard.cs
Hash: `91b9a85c69afb98f`

## `public sealed class LQuillCard`

The card edits of one tenure, each building one request.
It adds, removes and moves whole cards, and takes a card's typed title, expression and definition.
The list rules live in `LDraftClerkCard`, and this type only reads the draft and sends.

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

## `public void LCardTitleSet(long card, string text)`

Defers a card's typed title, so each keystroke joins one undo step.

## `public void LCardExpressionSet(long card, string text)`

Defers a card's typed expression, like its title.

## `public void LCardMeaningSet(long card, string text)`

Defers a card's typed definition, like its title.
