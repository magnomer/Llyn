# LPortraitSection.cs

## `public sealed record LPortraitSection`

One node of a page likeness, from a headed band down to a single example row.
Every export writer walks this one shape, so a new field costs one reader line.
It carries lines, chips, links, a Markdown note, media plates and child sections.
A writer draws whichever are present and recurses into the children.
The role names the display shape the node came from.
A writer dresses the node by role and never by guesswork.
A position above zero numbers a card, and zero leaves a section unnumbered.

**Parameters**

- `LPortraitSectionHeading` - the section heading, or the card title when numbered.
- `LPortraitSectionLine` - the labelled lines, such as one gloss per language.
- `LPortraitSectionNote` - Markdown drawn as blocks, empty when the section holds lines instead.
- `LPortraitSectionImage` - the picture plates.
- `LPortraitSectionVideo` - the video plates.
- `LPortraitSectionPosition` - the badge number of a card, zero when unnumbered.
- `LPortraitSectionChip` - the chips, such as situations, registers, tags or parts of speech.
- `LPortraitSectionLink` - the linked entries, such as translations.
- `LPortraitSectionChild` - the nested sections, such as child cards and example rows.
- `LPortraitSectionRole` - what the section is on screen, a band unless the reader says otherwise.

## `public static LPortraitSection LPortraitSectionCreate(string heading, string text)`

A section holding one unlabelled line.

## `public static void LPortraitSectionAdd(List<LPortraitSection> sections, string heading, LStateValue value, string mark)`

Appends a headed section for a state only when that state shows text.
A page hides a field that is unwritten and marks one that is unknown.

## `public static LPortraitSection LPortraitSectionCreate(string heading, IReadOnlyList<LPortraitLine> lines)`

A section holding labelled lines and nothing else.

## `public static LPortraitSection LPortraitSectionCreate(string heading, IReadOnlyList<LPortraitSection> children)`

A headed band holding child sections and nothing else.

## `public static LPortraitSection LPortraitSectionCreate(string heading, LPortraitRole role, IReadOnlyList<string> chips, IReadOnlyList<LPortraitLink> links)`

An unnumbered section of the given role holding chips or links and nothing else.
