# LPortraitPage.cs
Hash: `657609a913d4981f`

## `public sealed record LPortraitPage`

Format-neutral likeness of one read view, which is an entry, an example, a source or a situation.
It is laid out as those views are, with a title, reading lines and chips, then a tree of sections.
Every field is already the text the reader sees, so a renderer decides nothing.
A page that is printed or exported comes from here, never from the screen.

**Parameters**

- `LPortraitPageTitle` - the head of the page, already substituted when untitled.
- `LPortraitPageLanguage` - the language chip, empty when the view has none.
- `LPortraitPageChip` - the further chips beneath the title, such as a kind and a usage tally.
- `LPortraitPageSection` - the sections in reading order, each left out while unwritten.
- `LPortraitPageFavorite` - whether the star beside the title is filled.
- `LPortraitPageLine` - the reading rows under the title: pronunciation, respelling, transcription, reflex.

## `public static LPortraitPage LPortraitPageOpen(LPortraitPage page)`

The same page with every collocation row open, at any depth.
PDF and print show every row, because paper cannot unfold one.
An HTML export keeps the stored folds instead.
