# LRequestGloss.cs

The Gloss requests, one per gesture a shown sentence offers on its renderings.
Every one names the card and the sentence by id, real or minted.
`LRequestCardId` 0 and `LRequestSentenceId` 0 together name the draft's own Example instead, the one the corpus panel holds.
So one set of four records serves the card editor and the corpus panel alike.

## `public sealed record LRequestGlossAddition(`

Adds an empty Gloss in `LRequestLanguage` at `LRequestPosition`, clamped into the list.
The engine mints its negative id, so the form never carries one of its own.

## `public sealed record LRequestGlossRemoval(`

Drops one Gloss by id.
A Gloss no row carries is refused.

## `public sealed record LRequestGlossText(`

Replaces the text of one Gloss, as written with its mark.

## `public sealed record LRequestGlossLanguage(`

Replaces the language of one Gloss.
