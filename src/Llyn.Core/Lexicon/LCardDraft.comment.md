# LCardDraft.cs

## `public sealed record LCardDraft(`

One card of the input form, captured as an immutable value. A Meaning card and a Collocation card are the same card — Title, a meaning text, Example, Situation, Synonym and Tags — so they are the same value here, and the Collocation's own Expression field is the single member a Meaning card leaves empty. A field added to the form is therefore added once.

A draft is what the shell hands the engine before anything is persisted: it carries the typed text exactly as it stands on screen, plus the id of the row it was loaded from when it came from one — no state, no database concern beyond saying which stored card it is. Empty fields stay empty strings rather than `null`, because "nothing was typed" is what the form means. The card's position is not carried: it is the order of the list the card sits in, which the save reads off that order and the load writes back into it.

Example, Situation and Tag are ordered sets, because the store models them as many-per-card: a card may reference any number of each, and the order of the list is the order they are stored in and read back. A field the form leaves empty is an empty list, never a list holding an empty string. Turning what a control holds into such a list is the shell's job — the engine takes the list as given and never splits text into rows itself. Two cards holding equal lists are not equal values, because the generated equality compares those lists by reference; a comparison that means "the same card" walks the lists itself.

**Parameters**

- `LCardDraftTitle` — Title text from the card's Title field.
- `LCardDraftExpression` — Expression text from a Collocation card's Expression field; always empty for a Meaning card, whose template has no Expression control.
- `LCardDraftMeaning` — The card's meaning text: the Definition field of a Meaning card, the Meaning field of a Collocation card. One field, labelled differently on the two templates.
- `LCardDraftExample` — Example texts the card references, in the order they are shown.
- `LCardDraftSituation` — Situation titles the card references, in the order they are shown.
- `LCardDraftSynonym` — Always empty. Neither card template offers a Synonym control: a synonym is a link to a stored Entry or Meaning, not text the card owns, and it is written through the engine's relation seam against a target the caller resolved. The member is kept so the card shape stays one shape for both kinds.
- `LCardDraftTag` — Tag texts the card references, in the order they are shown.
- `LCardDraftId` — Id of the stored row this card was loaded from, empty for a card that has never been stored. It is the one member that is not typed text, and the only reason it exists is the update: a draft handed back for saving must say which stored Meaning or Collocation each card is, or an update could only match cards by their place in the list and would move one card's text onto another card's row. A card carrying no id is a new card and is created; the form builds its cards without one, so nothing in the shell has to carry it until it chooses to.
