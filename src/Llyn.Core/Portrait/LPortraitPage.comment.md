# LPortraitPage.cs

## `public sealed record LPortraitPage`

Format-neutral likeness of one read view that is not an entry: an example, a source or a situation.
It is laid out as those views are: a title at the head, chips beneath it, then headed sections.
Every field is already the text the reader sees, so a renderer decides nothing.
A page that is printed or exported comes from here, never from the screen.

**Parameters**

- `LPortraitPageTitle` - the head of the page, already substituted when untitled.
- `LPortraitPageLanguage` - the language chip, empty when the view has none.
- `LPortraitPageChip` - the further chips beneath the title, such as a kind and a usage tally.
- `LPortraitPageSection` - the headed sections in reading order, each hidden while unwritten.
