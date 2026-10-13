# Media

Media holds the pictures and films a Situation or card points at.
Each is independent data owned by nothing, and any number of Meanings and Collocations reference it.
The order it appears in lives on each reference rather than on the record.
The location is a file on this machine or an address on the web, and is never identity.
The store keeps the text it was given and resolves nothing, since a path unreachable today may be reachable tomorrow.
A row that named a picture is not the same as a row that never named one.
Correcting a path leaves the id and every reference untouched.

## `LImage`

One Image with an id and a location.
A Video is shaped exactly like an Image because it is kept exactly like one.

## `LImageDraft`

One image row a card is holding before it is stored.
A card points at an image rather than owning one, and two cards may point at the same picture.
Carrying the stored row id beside the location is what lets a citation stay a citation.
Without it a file that declared one picture twice would import as two rows.
The situation and register drafts carry their row id for the same reason.
`LImageDraftEmpty` is true when the row names no picture.

## `LVideo`

One Video with a location and a span of it worth watching.
The span is never identity either.
Correcting a span leaves the id and every reference untouched.

## `LVideoFilmRead`

The film id inside a hosted video address, or null when the address is not one.
Every shape the host hands out is read, since a user pastes whichever one they were given.
The video screen and the portrait sheet both ask here, so one rule names a film.
`LVideoFilmLength` is eleven, the length of every film id the host hands out.
`LVideoFilmCheck` refuses anything but eleven ASCII letters, digits, dashes and underscores.
The id is written into a page's script and address, so the check keeps that text closed.

## `LVideoSpanRead`

The moments a written span plays from and until.
The span is split at its dash, and each side is read as `mm:ss` or `hh:mm:ss`.
A start that reads as no moment plays from the beginning.
An end that is missing or unreadable plays to the film's end.
Half-typed text never fails, so a form can hold it while the screen still plays.
`LVideoMomentParse` reads two or three colon fields, each a plain count, or null for anything else.

## `LVideoDraft`

One video row a card is holding before it is stored.
The location and the span are both the user's writing, so they travel together from the form to the store.
The span is text and not a pair of moments, because half-typed text is what a form holds.
Refusing to carry it would lose what was typed.
`LVideoDraftEmpty` is true when the row names no film, and a span alone never keeps a row alive.

## Stamp

Commit: `c2f335c331fe`
- `LImage.cs` `4825abe1204c6769`
- `LImageDraft.cs` `84e14c7d69765300`
- `LVideo.cs` `99c10bad4f5c23b8`
- `LVideoDraft.cs` `73038fe3640b1b10`
