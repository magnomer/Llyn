# Sheaf

These tests pin the rules and workflows of flashcard cues.
The cue rule tests need no workspace.
The scan tests read a draft alone.
The other cue clerk tests run over a real workspace.

## `TCue`

Covers when a card is orphaned and when it rests.
A meaning card whose meaning is gone is orphaned, and a free card never is.
A pair needs its target and one of its meaning or collocation.
An excluded card rests even when nothing is gone.

## `TCueClerk`

Covers the read, scan, add, line and cell edit of cards, and the stencil read, slot set, save and apply.
A deck's card read returns only that deck's cards.
A scan returns every meaning at any depth, and one pair per translation target.
A second add of the same entry adds nothing.
The accession find lists catalog rows in order and marks only the entry the deck holds.
A free card reads back its saved cells, and a bound card refuses them.
A meaning card's line reads its definition, and a free card's line its first Text cell.
An orphaned card's line reads empty.
A new deck's template reads as its grain's default.
A slot set replaces a held slot in place and appends a new one.
A new deck owns an unsaved template and two bearings.
An applied stencil copies its slots, and a saved stencil keeps them through later edits.

## `TCueMarkup`

Covers the phone markup's handling of a literal bar.
A bar in text becomes a look-alike divider, so it cannot split a line.
Covers the study side of one face too.
A bar becomes a line break, and another side's cells stay out.
Picture and sound cells fill the side's file lists and never reach its text.

## `TCueStudy`

Covers the reads study practises from, at the clerk and through the deck port.
A resting card is left out of the faces and of the active counts.
A free card's front and back come from its own cells.
The active read answers folders, decks and counts together, and a deck with no active card has no count.
The census read counts only the chosen deck.
With no deck chosen, it takes the latest sync of any deck.
Its weeks are checked only by their sum, since the cards carry the real time of their insert.

## Stamp

Commit: `121691f60459`
- `TCue.cs` `fb984bcfc6fa43be`
- `TCueClerk.cs` `21f7b1c324e2d086`
- `TCueMarkup.cs` `23a86f25140dc5ab`
- `TCueStudy.cs` `ebf8ab3640357560`
