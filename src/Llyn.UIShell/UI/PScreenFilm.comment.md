# PScreenFilm.cs

## `public partial class PScreen`

Reading a YouTube film id out of an address.
It is kept apart from the page it is written into, because it is an answer about an address alone.

## `internal static string? PScreenFilmRead(Uri address)`

The film id inside a YouTube address, or `null` when the address is not one.
Every shape that site hands out is read, since a user pastes whichever one they were given.

## `private static string? PScreenFilmCheck(string film)`

An id holding anything but letters, digits, dashes and underscores is refused, because it is written straight into the page.
