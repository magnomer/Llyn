# Tag

This folder holds the Tag mark in its stored, draft, offer and row shapes.

## `LTag`

A Tag is a shared row any number of Meanings and Collocations link to by id.
Two Tags never read alike, because the store keeps the text unique.
A Tag carries no state of its own, and a nameless Tag does not exist.
So the same Tag can sit first under one Meaning and third under a Collocation.
Text may hold spaces and ordinary punctuation.
Only a comma separates one Tag from the next where Tags are written as a line.

## `LTagDraft`

The id of the Tag the row links and the text the row shows.
A positive id names a stored Tag, and the text is what was read back from it.
A zero id means the text was typed and no Tag has been resolved for it yet.
The save resolves such a row to the Tag reading the same, or creates one.

## `LTagOffer`

What a tag field keeps after a gate, and the stored Tags it offers.
The offered rows are at most eight, match the trimmed text, and leave out those the card holds.
It is shown only when the text is not blank and some stored Tag is offered.

## `LTagRow`

One stored Tag a tag field offers, its text already split around the typed word.

## Stamp

Commit: `c2f335c331fe`
- `LTag.cs` `4c045bfae2973c3e`
- `LTagDraft.cs` `a16e895a057c5664`
- `LTagOffer.cs` `d72f09d5ee506774`
- `LTagRow.cs` `9102f844e4387a6d`
