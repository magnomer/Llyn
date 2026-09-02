# TImage.cs

## `public sealed class TImage`

Covers the Images a card carries end to end, through the one path the form uses.
That is a draft saved, loaded back, and updated.
It asserts that the order a card gives its Images is the order they come back in.
It asserts that dropping one from a card detaches it.
It asserts that a location left blank is written nowhere, not stored as an empty picture.

## Inline notes

### `["D:\pictures\word.png", "https://example.com/word.png"]`

One card holding both kinds of location at once.
The store keeps the text and never asks whether the picture can be reached.
