# Sheet

This folder holds the tests of the HTML rendering of a portrait.
They also cover the entry sheet that HTML, PDF and print share.

## `TPortraitSheet`

Covers the HTML rendering, which the PDF is printed from.
What is proved here holds for both.
Only the collocation rows differ, since the PDF prints a page opened by `LPortraitPage.LPortraitPageOpen`.
A folded row on a page opened for print shows open under its list heading.
Its summary holds the phrase and the meaning, and its fields follow.
A folded row on an unopened page keeps its fold, as an HTML export shows it.
Its fields stay inside the closed disclosure.
A field silently dropped is the failure this export exists to avoid.
The palette is asserted against the theme rather than against literals, so a changed theme moves the page with it.
Entry text is written by the reader and may hold anything, so no unescaped tag may remain.
A document plays nothing, so a YouTube video must still show its poster frame and the link.

## `TSheetEntry`

Covers the entry sheet that HTML, PDF and print share, one fact per rule.
Each page is read from a stored entry and changed with `with` only where a fact needs it.
So every other member stays real.
The lookup answers each key as itself, so a label reads as its key.
Each fact asserts the fragment that carries the rule, never the whole document.

The body takes the surface colour and the entry takes the insets of the view pane.
A variety with a flag shows the flag with the variety as its title and no label.
A variety with no flag shows its name.
A language with no flag shows the globe beside its name.
The language and a variety sharing one flag embed it once, with one image rule that both places point at.
The print rules ask for exact colours and drop only the frame padding.
The page margin is set in its own `@page` rule, and print never forces a white background.

A grasp of three fills one star and half of the next, and leaves three empty.
The rating label beside the stars is the key of level three.
A grasp of twelve clamps to ten and shows the key of the top level.
An entry never rated fills no star and shows the key of level zero.
A ranked frequency writes one pill whose title lists the source and the figure.
The Everyday tier lights three of the four pips and leaves the last one dim.
A break row writes its part, its name and its shown text as three cells.
The part and the name are read from the row, since the row owns them.

A view whose short and full tables differ writes the switch with the short side checked.
An irregular run of a form is wrapped in the marked span.
A folded stored card writes closed details and an unfolded one writes open details.
A collocation linked to no meaning writes a row under the Other collocations heading.
The row sits in the collocations box and opens, since nothing folded it.
Its summary shows the phrase and then the sense.
The Other band is a plain band, never a deck of numbered cards.
A fanqie box the entry holds open writes open details under its heading.
The heading is read from the label the relay hands the writer, never typed.
The footer writes the created and updated stamps as a label and time grid.

### `TSheetEntryFlag`

A tiny inline image that stands for a flag file.
The sheet embeds it once however many places show it.

### `TLiveryVarietyCreate`

The stored page with a Brazil primary accent row and no other row.
The language banner is emptied, so only the variety can carry a flag.
The ensign map gives the variety its flag, or stays empty for a variety without one.

### `TSheetTextFormat`

Writes the sheet of a page beside the entry's portrait tree, with a lookup that answers each key as itself.
Without a page it uses the stored entry's own page.

### `TParadigmFormBuild`

Builds a paradigm cell whose first letters are marked, or with no mark when the count is zero.

## `TSheetSound`

Covers the sound section of the entry sheet that HTML, PDF and print share.
`TSheetEntry` keeps the sheet's other sections, so neither file outgrows its length limit.
The page is read from a stored Mandarin entry and given a toned contour with `with`.
The primary accent's reading and its plot share one `llyn-axis` block in one cell.
The reading comes first and the charts close the block, so the plot centres on the reading.
No separate contour row remains, and the cascade writes the axis as an inline-flex column.

## Stamp

Commit: `8c403d1df768`
- `TPortraitSheet.cs` `0100401a8287c7ce`
- `TSheetEntry.cs` `8d5452f0a0031a00`
- `TSheetSound.cs` `040fee8dc22722d3`
