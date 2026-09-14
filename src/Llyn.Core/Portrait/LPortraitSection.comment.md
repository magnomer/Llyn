# LPortraitSection.cs

## `public sealed record LPortraitSection`

One headed section of a page likeness.
It carries lines, a Markdown note, or media plates, and a renderer draws whichever are present.

**Parameters**

- `LPortraitSectionHeading` - the section heading.
- `LPortraitSectionLine` - the labelled lines, such as one gloss per language.
- `LPortraitSectionNote` - Markdown drawn as blocks, empty when the section holds lines instead.
- `LPortraitSectionImage` - the picture plates.
- `LPortraitSectionVideo` - the video plates.

## `public static LPortraitSection LPortraitSectionCreate(string heading, string text)`

A section holding one unlabelled line.

## `public static LPortraitSection LPortraitSectionCreate(string heading, IReadOnlyList<LPortraitLine> lines)`

A section holding labelled lines and nothing else.
