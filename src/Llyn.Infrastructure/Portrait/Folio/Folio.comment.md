# Folio

This folder writes a page likeness as a Word document.
The package is built directly rather than through a document library, so the project takes no new dependency.
Only the small part of the format Word needs to open a file is written.
It stands apart from Sheet because the output is an Office package, not HTML.

## `LFolio`

The body is built first, because building it is what discovers which pictures are embedded.
The content types and the relationships both depend on that list, so they are written after.
Word rejects a byte order mark in a package part, so the text encoding writes none.

## `LFolioBody`

The plate list is filled while the body is written, and the caller packs what it collects.
The drawing namespaces are declared once on the root, because every picture run uses them.
The title heads the document, with the star after it when the page is a favourite.
The language, reading lines and chips share one sound line, each reading led by its label.
A reading's open and close marks are written plainly around its text.
The sections follow in the order the likeness carries them, through `LFolioSection`.
The page is A4 with even margins, matching the printed PDF.

## `LFolioBond`

It builds the two parts that tie a Word package together.
`LFolioBondRead` writes the relationships, naming the styles and every embedded picture.
Picture ids start high so they never collide with the fixed ones.
`LFolioBondCreate` writes the content types, which must declare every extension the package holds.
Word refuses to open a package whose picture extension is undeclared.

## `LFolioLine`

It writes the paragraph shapes a Word body is made of.
A line break and a tab cannot ride inside a text run.
Each closes the run and opens a new one.
A carriage return is dropped, because Word takes the break from the element rather than the character.
A lead overload writes a bold lead followed by plain text.
The card number, the example bullet and the video marker take this shape.
`LFolioLineDraw` places an inline picture sized in the units the format measures documents in.
The relationship id is derived from the plate's place.
So the body and the relationships agree without being passed a map.

## `LFolioNote`

It renders the note's Markdown blocks as paragraphs under the note styles `LFolioStyle` declares.
A list item is a hanging-indent paragraph led by a bullet or its number, one indent step per level.
Numbers count up while consecutive items stay on one level and restart otherwise.
A rule is an empty paragraph with a bottom border.
Code keeps its line breaks through `LFolioLine`.
Spans become runs carrying bold, italic, monospace and underline.
A link whose target differs from its text is followed by the target in parentheses.

## `LFolioSection`

It renders one section node as paragraphs and recurses into its children.
The role of the node decides its style, so a situation, a register and a tag each keep their own.

### `LFolioSectionAppend`

A kind joins the card, a usage has its own rows, and any other role is a band.
A bridge draws nothing of its own, since the shared body already draws links.
A collocation list draws its heading as a label line, then its rows.
A row draws its heading as a phrase line, then its sense lines, then the shared body.
The folio reads no fold, so a folded row prints open.

### `LFolioBandAppend`

A band is a heading, the chips as one sound line, one row per line, then the shared body.
The chips take the sound style because the only band chips are parts of speech and characters.
Each line starts with its opener, so an etymology line keeps its depth dots and arrow.

### `LFolioCardAppend`

A card is a one-column table.
A shaded header row holds the number and heading, then one cell holds everything else.
The header is muted when the heading is only the card's kind.
A phrase standing first among the children is drawn before the sense lines, as the panel shows the expression.
Each line is a sense paragraph led by its bold label when it has one.
A cell must end in a paragraph, so an empty one closes it before the table closes.

### `LFolioQuoteAppend`

A quote is a bulleted paragraph whose bold lead is the first line's label.
The further lines follow as indented detail rows, then the chips, then the shared body.

### `LFolioUsageAppend`

One row per link carries the arrow and headword.
One detail row follows with the line texts, their labels and the link languages.

### `LFolioBodyAppend`

The shared body is the bridge line, the note, the children from `start`, then the plates.
The children come before the plates because the panel draws its pictures under the tags.
A readable picture is embedded at its pixel size, narrowed to the text width when wider.
An unreadable one falls back to its address, and a video is a play mark before its address.

### `LFolioPlateClamp`

Word measures in EMUs, so pixel sizes are scaled and clamped to the column width.
An unreadable size takes a widescreen box, so the picture still has a place.

## `LFolioStyle`

It builds the style part from the display's theme.
Each display role becomes one named style, so the body carries no inline formatting.
Sizes are the panel's own, converted from device-independent pixels to half-points.
A register line takes the muted tone style, as the panel outlines a register chip in grey.
The note adds a heading, quote and code style, since its Markdown carries those shapes.
`LFolioStyleNormalize` takes six hex digits with no leading hash and no alpha.
An unknown colour falls back to black rather than breaking the part.
Spacing after a paragraph carries the panel's vertical rhythm, which the format has no margins for.

## Stamp

Commit: `c6e60141a172`
- `LFolio.cs` `0a4966817b9beb2f`
- `LFolioBody.cs` `f6b99d3695b0cd1d`
- `LFolioBond.cs` `464c860803671ef2`
- `LFolioLine.cs` `47a15cb22bc534ba`
- `LFolioNote.cs` `a0a3e168b71cc3fa`
- `LFolioSection.cs` `4df34e40e6e3c439`
- `LFolioStyle.cs` `8bda335bbda1784a`
