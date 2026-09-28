# LPortraitFilm.cs

## `public static class LPortraitFilm`

Recognizes a hosted video address.
A hosted video is the one kind whose poster frame can be named without decoding anything.

## `public static string? LPortraitFilmRead(string location)`

The written location is read as an address, and Core's `LVideoFilmRead` names the film in it.
The video screen asks the same rule, so both accept the same address shapes.
A local file yields nothing, because no frame can be taken from it without a decoder.
