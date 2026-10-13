# Sheet

This folder renders a page likeness as standalone HTML.
It holds the catalog page writer, its style, and the section, note and plate writers.
The HTML is the fidelity anchor of the whole export, and the PDF is this page printed.
It stands apart from Folio because the output is text markup.
The entry sheet in the `Entry` subfolder reuses its section writers and its palette.

## `LSheet`

It renders a page likeness as one standalone HTML page.
Every catalog page prints in this one dress, and an entry with its livery takes the entry sheet instead.
Anything fixed here is fixed in both the HTML and the PDF.
`LSheetFormat` inlines the style sheet and embeds pictures, so the file stands alone once moved.
Nothing is decided here, since an absent section was already left out by whoever built the likeness.
`LSheetBodyAppend` writes the visible content inside one `main` element.
`LSheetFormat` wraps it in the document that carries the style sheet.
`LSheetNormalize` is the gate every written value passes, since entry text may hold markup.

### `LSheetCrestAppend`

The title, language chip and star form the masthead.
Each reading line follows as a sound row, its label muted before its text.
The line's open and close marks are drawn around the text in their own muted style.
The further chips share one row beneath, and no row is written when there are none.

## `LSheetStyle`

It is the appearance of a catalog sheet, built from the display's theme.
An entry sheet takes `LSheetCascade` instead.
Colours become custom properties so one declaration feeds every rule.
Sizes, margins and radii are the display's own values in device-independent pixels.
Backgrounds are forced to print, because a card without its ground is not the card seen.
Cards are kept off page breaks, which the panel never has to think about.

### `LSheetVariableFormat`

It answers the `:root` rule naming each theme font stack and each colour in the spare set.
The contour colours stay out, since each chart image carries its own.
The catalog style and `LSheetCascade` both open with it, so the two sheets share one palette.
Each property keeps the theme file's key in kebab case, so `surfaceRaised` becomes `--surface-raised`.

### Build

`LSheetStyleBuild` writes the properties, then every shared rule, then `LSheetStylePaper`.
`LSheetVariableAppend` writes one property with the theme text untouched.

## `LSheetSection`

It renders one section node as HTML and recurses into its children.
The role of the node decides its dress, so a situation, a register and a tag keep their own styles.

### `LSheetSectionAppend`

Card, kind, phrase, scene, tone, label, bridge, quote, usage, list and row each have a form.
A collocation list is a bordered `collocations` block headed by its count, holding its rows.
Any other role is a band.
A bridge draws nothing of its own, since the shared body already draws links.

### `LSheetSectionFind`

It finds the first top-level section under a heading, or null.
The entry sheet picks its bands by the label's heading, since the portrait carries no other key.

### Bands, cards and senses

A band has its chips as a speech row, then its lines, then the shared body.
A band of usage rows wraps them in one rows block, so the row style stacks them.
A band of collocation rows wraps them in the box a homed list draws.
A card header holds the badge number and heading, the heading muted when it is only the card's kind.
`LSheetSenseAppend` writes a card's body without its frame, so the catalog card and the entry deck's card share it.
A phrase standing first among the children is drawn before the sense lines, as the panel shows the expression.
Each line is a sense paragraph, with its label as a small tag when it has one.

### `LSheetRowAppend`

A collocation row is a `collocation` disclosure whose summary holds the phrase and the meaning.
The row is open unless the section fold closes it, so an HTML export keeps the stored fold.
PDF and print hand in a page opened by `LPortraitPage.LPortraitPageOpen`, so paper shows every row.

### Quotes, usages and body

A quote is a bulleted row whose first line is the quote.
Its label is the frame and its text is what was said.
The further lines follow as tagged lines, the chips as labels, then the shared body.
The parent opens the enclosing quotes block, so neighbouring quotes share one.
A usage is one entry that links here, drawn as the display does.
It has an arrow, the headword over the card title, the kind as a pill and the language muted.
The shared body is the links as a bridge row, the note as Markdown blocks, the children, then the plates.
A run of quote children is wrapped in one quotes block, as the panel groups a card's examples.
The children come before the plates because the panel draws its pictures under the tags.
A tag is written only when the label is not empty.
A chip row is skipped when there are no chips.

## `LSheetNote`

It renders the note's Markdown blocks as HTML inside the sheet's note box.
`LSheetNoteAppend` writes one element per parsed block.
Consecutive items of one kind share a list, and a deeper item opens a nested list inside the last.
A heading is written two levels down, so the note never outranks the sheet's own section headings.
A line break inside a block becomes `<br>`.
Code text is escaped and nothing else, so a fence shows exactly what was typed.
Spans nest link, strong and em around each piece.

## `LSheetPlate`

It renders the picture and video plates a section holds.
A local picture is embedded, so the page keeps its images after it is moved or mailed.
A picture that cannot be found takes a plain plate, so the layout keeps its place without a broken mark.
A video cannot play in a document, so it becomes a poster frame over a link.
A hosted video lays the host's own thumbnail over the plain plate, which shows through if the host is unreachable.
`LSheetAddressRead` yields an address an image tag can carry, or null.
A readable local file or a data address qualifies, and becomes a data address the page carries within itself.
A web address is dropped, or the page would fetch from a host the entry named, wherever it is opened.
A local path that does not read is dropped too, since elsewhere it would point at nothing.

## Stamp

Commit: `c2f335c331fe`
- `LSheet.cs` `290b8d5803491019`
- `LSheetNote.cs` `2b13b12ff8bb3151`
- `LSheetPlate.cs` `97f2595914cddff6`
- `LSheetSection.cs` `01d55bde8e3f7b27`
- `LSheetStyle.cs` `a011f8b916bf3a3b`
