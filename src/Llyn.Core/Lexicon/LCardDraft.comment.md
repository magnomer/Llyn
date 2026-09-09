# LCardDraft.cs

## `public sealed record LCardDraft(`

One card of the input form, captured as an immutable value.
A Meaning card and a Collocation card are the same card.
Both carry Title, a meaning text, Example, Situation, Register, Translation, Synonym and Tags.
So they are the same value here.
The Collocation's own Expression field is the single member a Meaning card leaves empty.
A field added to the form is therefore added once.

A draft is what the shell hands the engine before anything is persisted.
It carries the typed text exactly as it stands on screen.
It also carries the id of the row it was loaded from when it came from one.
There is no state, and no database concern beyond saying which stored card it is.
Empty fields stay empty strings rather than `null`, because "nothing was typed" is what the form means.
The card carries the number it is shown by, so no view has to count the list itself.
The save still ignores it and rewrites every position from the order of the list.
The load fills it from the stored position, which is the same order read back.

Example, Situation, Register, Translation, Tag and Image are ordered sets, because the store models them as many-per-card.
A card may reference any number of each.
The order of the list is the order they are stored in and read back.
A field the form leaves empty is an empty list, never a list holding an empty string.
Turning what a control holds into such a list is the shell's job.
The engine takes the list as given and never splits text into rows itself.
Two cards holding equal lists are not equal values.
The generated equality compares those lists by reference.
A comparison that means "the same card" walks the lists itself.

**Parameters**

- `LCardDraftTitle` — Title from the card's Title field, and what is known about it.
  Nothing was recorded when the field stands empty.
  It is unreadable when it holds something that cannot be read back.
- `LCardDraftExpression` — Expression text from a Collocation card's Expression field.
  It is always empty for a Meaning card, whose template has no Expression control.
- `LCardDraftMeaning` — The card's meaning text: the Definition field of a Meaning card, the Meaning field of a Collocation card.
  One field, labelled differently on the two templates.
- `LCardDraftExample` — The Examples the card references, in the order they are shown.
  Each carries its id, its sentence and the Source it cites.
- `LCardDraftSituation` — The Situations the card references, in the order they are shown.
  Each carries its id, its wording and the Source it cites.
- `LCardDraftRegister` — The Registers the card is marked with, in the order they are shown.
  Each carries its id and its wording, the way a Situation row does.
  A Register is shared data, so two cards marked the same way reference one stored row.
  The wording a chip shows is read back from that row, so a renamed Register follows.
  A row a language pack ships and a row the user wrote sit in the same list.
- `LCardDraftTranslation` — The Entries this card is translated by, in the order they are shown.
  Each is the id of a stored Entry and never the word it shows.
  The word a chip shows is read back from that Entry, so a renamed headword follows.
  A link crosses languages, so a target sits in a language pack the card's own entry does not.
  The list is one way: the target holds nothing pointing back.
- `LCardDraftSynonym` — Always empty.
  Neither card template offers a Synonym control.
  A synonym is a link to a stored Entry or Meaning, not text the card owns.
  It is written through the engine's relation seam against a target the caller resolved.
  The member is kept so the card shape stays one shape for both kinds.
- `LCardDraftTag` — The Tags the card carries, each one its own text, in the order they are shown.
  A Tag is its name, so the list holds plain text and no Tag is ever blank.
- `LCardDraftImage` — The Images the card references, in the order they are shown.
  Each carries the location it is loaded from, a file on this machine or a web address.
  Each also carries what is known about that location.
- `LCardDraftVideo` — The Videos the card references, in the order they are shown.
  Each carries the location it is loaded from, a file on this machine or a web address.
  Each also carries what is known about that location.
  A Video is kept with the card exactly as an Image is.
- `LCardDraftPosition` — The number this card is shown by, counted from one.
  It is the reader's number and never the offset the store keeps.
  The store counts from zero, so the load adds one on the way out.
  There is no default, so every caller states the number the card stands at.
  A card the engine saves has its number rewritten from the order of the list it sits in.
- `LCardDraftId` — Id of the stored row this card was loaded from.
  It is empty for a card that has never been stored.
  It is the one member that is not typed text, and it exists only for the update.
  A draft handed back for saving must say which stored Meaning or Collocation each card is.
  Otherwise an update could only match cards by their place in the list.
  That would move one card's text onto another card's row.
  A card carrying no id is a new card and is created.
  The form builds its cards without one, so nothing in the shell carries it until it chooses to.
