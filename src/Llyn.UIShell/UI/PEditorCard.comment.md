# PEditorCard.cs

## `public partial class PEditor`

The two card lists of the form, built from stored cards and read back off the controls.
A Meaning card and a Collocation card are the same card, so one pair of paths serves both lists.
The panel owns this and not the card, because only the panel knows which list a card stands in.

## Inline notes

### `private void PCardShow(`

The inverse of PCardRead, for either list.
Each card takes the number the draft carries, not its place in the loop.
Each card takes the id the draft carries too, which is how a later answer names it back.
An entry saved with no cards still shows one empty card.
That card is minted like any other the form adds, so it is named before anything is typed into it.
The panel is an editor.
An editor with nothing to type into is not a state the form has.

### `PCardId = draft.LCardDraftId,`

Which stored row this card is.
It is carried through the form untouched.
So a save of the same card changes that row instead of adding another one beside it.

### `PCardDraft = draft,`

The whole draft the card was drawn from, kept beside the fields drawn out of it.
The read writes the form's fields over it rather than building a card from nothing.
So every column the form has no control for goes back exactly as it came.

### `card.PCardLinkShow(PCardTargetRead(targets, draft.LCardDraftTranslation));`

Each card takes its own words out of the one answer, in the order the card holds them.
An id the answer does not name is passed over, as a chip with no Entry has nothing to say.

### `PLinkAttach(card);`

Every card is given the way back to the editor before it is shown.
A card resolves no typed word on its own.

### `PEditorChangeAttach(card);`

A card that cannot report its own edits would be typed into without ever being written.

### `private IReadOnlyList<LCardDraft> PCardRead(IReadOnlyList<PCard> cards)`

The one read path for both card lists.
A Meaning card and a Collocation card are the same card.
So they are read into the same value.
A Meaning card's Expression stays empty.
Its template has no Expression control, and no writer looks at the field for a meaning.
The Translation line reads back the ids the card's link field holds.

### `LCardDraftTitle = card.PCardTitleRead(),`

The card's own Title field, which is not PCardTitle.
That one is the "Meaning 1" header the template shows as a placeholder over this box.

### `LCardDraftMeaning = card.PCardDefinitionRead(),`

The Meaning field of a collocation card and the Definition field of a meaning card are one property.
One card class serves both kinds, so the label differs and not the field.

The Synonym line is not written over at all.
Neither template has a Synonym control any more.
A synonym is a link to a stored Entry or Meaning, and no picker resolves typed text to one.
So the field is not offered, rather than offered and discarded.

### `private static LCardDraft PCardDraftCreate()`

The empty card a form with no stored draft behind it reads back.
Every list is empty and every field unwritten, which is what a blank card means.
It exists so the read has one shape to write the form over, stored or not.
