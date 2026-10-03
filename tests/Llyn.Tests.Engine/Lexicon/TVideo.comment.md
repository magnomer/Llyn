# TVideo.cs
Hash: `2884e3c748bec951`

## `public sealed class TVideo`

Covers Core's rule that names the hosted film inside a video address.
It also covers the rule that reads a written span into a start and an optional end.
A span that cannot be read answers the whole film.

## `public void VideoFilmRead_HostedShape_AnswersTheFilmId(string address)`

Every address shape the host hands out yields the same eleven-character id.
The host name is read without case and without its `www.` label.

## `public void VideoFilmRead_OtherAddress_AnswersNothing(string address)`

Another host, an unknown path and a local file answer null.
An id of the wrong length or with a stray character answers null too.
The character check matters because the id is written into a page's script.

## `public void VideoSpanRead_WrittenSpan_AnswersItsMomentsOrTheWholeFilm(string span, int from, int? until)`

A written span reads as a start and an optional end, each in `m:ss` or `h:mm:ss` form.
A lone moment gives no end, and an unreadable start gives zero.
The player is told these moments, so a bad span must play the film rather than refuse.
