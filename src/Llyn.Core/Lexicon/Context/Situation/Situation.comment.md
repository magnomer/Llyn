# Situation

This folder holds the Situation mark in its stored, draft, offer and row shapes.

## `LSituation`

The usage context a Meaning or Collocation belongs to.
No Entry, Meaning or Collocation contains a Situation.
A Situation cites nothing, since it is a description the user writes, not a passage quoted from a work.
Its images and videos are independent records too, referenced in the order the reference holds.
So a Situation reaches its media exactly as a Meaning does.
`LSituationImageFilled` leaves out a row the user added but never gave a location.
So a reading page draws no blank picture.
`LSituationVideoFilled` leaves out a blank row the same way.
Equality compares the Image and Video lists row by row, because comparing by reference would make every read a change.
`LSituationNormalize` also normalizes each Image and Video row.

## `LSituationDraft`

The id of the Situation the row edits and the three fields the store keeps.
The draft never owns them, because the id is the reference.
An empty id means the row has not been stored as a Situation yet.
The card shows the title alone, and the description and kind ride along untouched.
They are stored data, so a save that dropped them would erase what another panel wrote.

## `LSituationOffer`

What a situation field keeps after a gate, and the stored Situations it offers.
The offered rows are at most eight, match the trimmed text, and leave out those the card links.
It is shown only when the text is not blank and some stored Situation is offered.

## `LSituationRow`

One stored Situation a situation field offers, its title already split around the typed word.
The count is how many cards already use the Situation, empty when none does.

## Stamp

Commit: `c2f335c331fe`
- `LSituation.cs` `3fc8114fa6850e39`
- `LSituationDraft.cs` `c7ccc92d37a07b35`
- `LSituationOffer.cs` `d82e5cd8b08662eb`
- `LSituationRow.cs` `d6494e9dc89cf02b`
