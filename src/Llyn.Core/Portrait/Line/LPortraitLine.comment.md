# LPortraitLine.cs

## `public sealed record LPortraitLine`

One line of a page section: an optional label and the text beside it.
A gloss is a line whose label is its language.

**Parameters**

- `LPortraitLineLabel` - the label before the text, empty when there is none.
- `LPortraitLineText` - the text the reader sees, bare of any bracket.
- `LPortraitLineOpener` - the mark a writer draws before the text, such as `[` or `/`, or empty.
- `LPortraitLineCloser` - the mark drawn after the text, or empty.
