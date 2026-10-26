# Rack

These tests pin the rules and workflows of flashcard decks.
Each Core rule test drives one rule through a relay, with no workspace.
The deck clerk tests run over a real database.
Only SQLite enforces the deck place and the card grain checks.

## `TDeckCode`

Covers the codes that keep deck file names apart.
A taken name gains ` 2`, then ` 3`.
A match ignores case, non-ASCII letters included.

## `TDeckCensus`

Covers the census counts and the latest sync.
A card counts in every state it has, so an excluded orphan counts as both.
A stamp that does not parse is no sync.

## `TDeckWeek`

Covers the week edges: Monday midnight opens a week, Sunday's last second closes the one before.
A zone nine hours ahead puts a UTC Sunday evening on the local Monday.
A stamp before the span, after now's week or unreadable counts nowhere.

## `TDeckClerk`

Covers the deck and folder workflows over a real database.
A folder delete appends its decks at the top level, even where places would clash.
A rename keeps the code, and a taken name suffixes a new deck's code.
An update refuses a code another deck holds in another case.
An update that keeps the deck's own code saves.

## Stamp

Commit: `c37ca1837ded`
- `TDeckCensus.cs` `2338a466b0c97370`
- `TDeckClerk.cs` `11940141f47aaa8e`
- `TDeckCode.cs` `97c6a001d711f683`
- `TDeckWeek.cs` `d8fa43ef09eaaaf7`
