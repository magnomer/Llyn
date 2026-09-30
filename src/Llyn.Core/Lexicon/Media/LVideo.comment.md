# LVideo.cs

## `public sealed record LVideo(`

One Video — independent data owned by nothing.
No Entry, Meaning, or Collocation contains a Video.
Any number of Meanings and Collocations *reference* it instead.
The order a Video appears in lives on each reference rather than here.
`LVideoId` is the identity, an opaque and program-generated stable id.
`LVideoLocation` says where the film is read from and is never identity.
`LVideoSpan` says which stretch of it is worth watching and is never identity either.
Correcting a path or a span leaves the id and every reference to it untouched.

A Video is shaped exactly like an Image because it is kept exactly like one.
The location holds a file on this machine or an address on the web.
The store keeps the text it was given and resolves nothing.
A row that named a film is not the same as a row that never named one.

**Parameters**

- `LVideoId` — Opaque, program-generated stable id.
- `LVideoLocation` — Where the film is read from, a file path or a web address.
  It also carries what is known about that location.
  A Video that is there but unknown is not a Video that was never written.
- `LVideoSpan` — The stretch worth watching, written as `mm:ss - mm:ss`.
  The store keeps the text and reads no moments out of it.
  A span that says nothing is a film watched whole.

## `private const int LVideoFilmLength = 11;`

Every film id the host hands out is eleven characters long.

## `public static string? LVideoFilmRead(Uri address)`

The film id inside a hosted video address, or `null` when the address is not one.
Every shape the host hands out is read, since a user pastes whichever one they were given.
The video screen and the portrait sheet both ask here, so one rule names a film.

## `public static (TimeSpan, TimeSpan?) LVideoSpanRead(string span)`

The moments a written span plays from and until.
The span is split at its dash, and each side is read as `mm:ss` or `hh:mm:ss`.
A start that reads as no moment plays from the beginning.
An end that is missing or unreadable plays to the film's end.
Half-typed text never fails, so a form can hold it while the screen still plays.

## `private static TimeSpan? LVideoMomentParse(string moment)`

One moment of two or three colon fields, each a plain count, or null for anything else.

## `private static string? LVideoFilmCheck(string film)`

An id is eleven ASCII letters, digits, dashes and underscores, and anything else is refused.
The id is written into a page's script and address, so the check keeps that text closed.
