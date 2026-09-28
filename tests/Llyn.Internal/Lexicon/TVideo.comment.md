# TVideo.cs

## `public sealed class TVideo`

Covers Core's one rule that names the hosted film inside a video address.

## `public void VideoFilmRead_HostedShape_AnswersTheFilmId(string address)`

Every address shape the host hands out yields the same eleven-character id.
The host name is read without case and without its `www.` label.

## `public void VideoFilmRead_OtherAddress_AnswersNothing(string address)`

Another host, an unknown path and a local file answer null.
An id of the wrong length or with a stray character answers null too.
The character check matters because the id is written into a page's script.
