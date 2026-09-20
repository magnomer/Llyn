# PScreenFilm.cs

## `public partial class PScreen`

Reading a YouTube film id out of an address.
It is kept apart from the page it is written into, because it is an answer about an address alone.

## `internal static string? PScreenFilmRead(Uri address)`

The film id inside a YouTube address, or `null` when the address is not one.
Every shape that site hands out is read, since a user pastes whichever one they were given.

## `private static string? PScreenFilmCheck(string film)`

An id is eleven characters of ASCII letters, digits, dashes and underscores, and anything else is refused.
It is written straight into the page's script, so the check is what keeps that literal closed.
