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

## Stamp

Commit: `2f9a06b8eb95`
- `TCue.cs` `fb984bcfc6fa43be`
- `TCueClerk.cs` `21f7b1c324e2d086`
- `TCueMarkup.cs` `a2c80ee2e3b16c79`
