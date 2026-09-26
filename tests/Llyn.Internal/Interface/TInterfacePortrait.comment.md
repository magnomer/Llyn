# TInterfacePortrait.cs

## `internal static partial class TInterface`

The relay for the export surface.
Tests call production operations only through here, so a renamed operation breaks in one file.

`TPortraitTextRead` lists every string a page shows, in reading order, children recursed.
A writer that carries the page whole prints every one of them.
So a coverage test reads its expectations here and never from the draft.
A band or card prints its heading, and empty strings are skipped.
An image is a picture and not text, so its address and caption are not listed.
A video is a link drawn as text, so its address and span are.
