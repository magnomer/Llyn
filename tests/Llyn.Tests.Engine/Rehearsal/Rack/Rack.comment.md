# Rack

These tests pin the rules and workflows of flashcard decks.
Each Core rule test drives one rule through a relay, with no workspace.
The deck clerk tests run over a real database.
Only SQLite enforces the deck place and the card grain checks.

## `TDeckCode`

Covers the codes that keep deck file names apart.
A taken name gains ` 2`, then ` 3`.
A match ignores case, non-ASCII letters included.

## `TDeckClerk`

Covers the deck and folder workflows over a real database.
A folder delete appends its decks at the top level, even where places would clash.
A rename keeps the code, and a taken name suffixes a new deck's code.
An update refuses a code another deck holds in another case.
An update that keeps the deck's own code saves.

## Stamp

Commit: `2f9a06b8eb95`
- `TDeckClerk.cs` `11940141f47aaa8e`
- `TDeckCode.cs` `97c6a001d711f683`
