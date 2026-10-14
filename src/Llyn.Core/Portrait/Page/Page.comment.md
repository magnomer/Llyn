# Page

This folder holds the format-neutral shapes of a page likeness and the theme its writers draw from.
It stands apart from `Line`, which holds the builders that fill these shapes.
Every field is already the text the reader sees, so a renderer decides nothing.

## `LPortraitMedium`

The file formats an entry can be exported to.
Markup is the only one that reads back through the importer.
The other four are readings rather than records, and carry only what the display shows.

## `LPortraitPage`

Format-neutral likeness of one read view, which is an entry, an example, a source or a situation.
It is laid out as those views are, with a title, reading lines and chips, then a tree of sections.
A page that is printed or exported comes from here, never from the screen.
The title is already substituted when untitled.
The language chip is empty when the view has none.
Sections are in reading order, each left out while unwritten.

## `LPortraitPageOpen`

The same page with every collocation row open, at any depth.
PDF and print show every row, because paper cannot unfold one.
An HTML export keeps the stored folds instead.

## `LPortraitRole`

What one section of a page likeness is, so a writer draws it by role and never by shape.
A writer that guessed from depth, position and line count drew a situation and a tag the same way.
The role names the display shape the section came from, and every writer keeps one style per role.
Band is a headed group at the top of the page.
Card is a numbered card headed by its own title.
Kind is a numbered meaning card with no title, headed by its kind word.
Phrase is the expression of a card, one line.
Scene holds situation chips, and tone holds register chips.
Bridge holds linked entries, such as translations and mentions.
Quote is one example sentence with its frame, glosses, source and mentions.
Label holds tag chips and the language chip of an example.
Usage is one entry that links here.
Its headword and language are the link, and the referring card is the line.
List is the collocations homed under a meaning, headed by their count, holding one row each.
Row is one collocation, with the phrase as heading, the meaning as line, then every filled field.

## `LPortraitSection`

One node of a page likeness, from a headed band down to a single example row.
Every export writer walks this one shape, so a new field costs one reader line.
It carries lines, chips, links, a Markdown note, media plates and child sections.
A writer draws whichever are present and recurses into the children.
A writer dresses the node by role and never by guesswork.
The role is a band unless the reader says otherwise.
A position above zero numbers a card, and zero leaves a section unnumbered.
The note is drawn as blocks and is empty when the section holds lines instead.
The fold is true when the row's fold is stored, so a writer that folds shows it closed.
It is false by default, and `LPortraitSectionOpen` clears it for print.
`LPortraitSectionLineage` is the etymology tree, null in every other section.
It is one `LLineage` box holding the headword, with the roots as its sources.
So the tree needs no Core type of its own.
The headword box has no id, entry, language, gloss or relation.
A writer that draws boxes reads the tree, and one that draws text reads the lines.

## `LPortraitSectionOpen`

The same section with its fold and every descendant's fold cleared.
`LPortraitPage.LPortraitPageOpen` is its one reader.

## `LPortraitSectionAdd`

Appends a headed section for a state only when that state shows text.
A page hides a field that is unwritten and marks one that is unknown.

## `LPortraitSectionCreate`

Overloads make a section of one unlabelled line, of labelled lines only, or of child sections only.
Another makes an unnumbered section of a given role holding chips or links only.

## `LPortraitFrame`

Builds the bracketed head that stands before an example sentence.
The frame belongs to the card's hold on the Example, so it is read off that row.
The particle and the dependence are placed in the order the language declares.
An empty pair yields no head at all, so no lone brackets are drawn.
The result matches what the display puts left of the sentence.

## `LTheme`

The display's colour table, held where every ring can read it.
The infrastructure loads it from the embedded theme file.
The exporters and the shell both draw from the one record.
So an exported page cannot drift from the panel it copies.
It takes the colours as read, and they are empty when the loader found nothing.

## Font stacks in `LTheme`

The family stack matches the shell's own family with web fallbacks appended.
The serif stack matches the serif the display uses for sentences.
The phonetic stack matches `Theme.Phonetic` in `PThemePalette.xaml`.

## `LThemeColorRead`

The colour the theme file names, refused when the file lacks it.
The shell fills its resources through this, since a panel drawn from a spare colour would be wrong quietly.

## `LThemeRead`

The name is the theme file's own key, not a resource key.
An unknown name yields the spare value, and black when even that is absent.

## Inline notes

### LTheme.cs: `private static readonly IReadOnlyDictionary<string, string> LThemeSpare =`

Each spare colour copies the key and value of `themes/default.json`.
It covers every key an exported sheet reads, so a thin theme file still paints the sheet as the panel.

## Stamp

Commit: `c6e60141a172`
- `LPortraitFrame.cs` `897bc6f64abf68e4`
- `LPortraitMedium.cs` `dc200723d24a5539`
- `LPortraitPage.cs` `657609a913d4981f`
- `LPortraitRole.cs` `9c2b6cdd51509e4a`
- `LPortraitSection.cs` `a9630333682168be`
- `LTheme.cs` `287bd1b58804b690`
