# LPortraitFilm.cs

## `public static class LPortraitFilm`

Recognizes a hosted video address.
A hosted video is the one kind whose poster frame can be named without decoding anything.

## `public static string? LPortraitFilmRead(string location)`

The same address shapes the video screen accepts are accepted here.
A local file yields nothing, because no frame can be taken from it without a decoder.

## `private static string? LPortraitFilmCheck(string film)`

An id may only hold the characters the host uses.
Anything else would put unchecked text into a page address.
