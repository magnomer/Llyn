# Markup

These tests pin the markup format, its reader and its writer.
They stand apart from the portrait tests because they test the entry text and its node tree, not rendered pages.
`TMarkupSample` is the one shared document, so a new element is added there once.

## `TMarkupSample`

One document that exercises every element the format knows.
The parse and format tests read it.
The text holds one entry with every row kind, a nested meaning, a collocation and a note.
The reflex row carries a respelling beside its text, as the pronunciation row does.

## `TMarkupFormatting`

Covers the markup writer through the reader.
A parsed entry written and read again is the same entry, and the text written twice is the same text.
Record equality covers every field, and text equality shows the writer is stable on its own output.
An unspecified value leaves no element behind, and the text carries no XML declaration.
An empty element would read back as a stated empty value, which the entry never held.
An unknown value leaves a marked empty element with a state attribute of unknown.
That attribute is the only way the reader tells unknown apart from unspecified.
The writer strips what XML cannot carry, so the text parses again instead of the export throwing.
An empty spelling and no spelling are different values, and the round trip keeps them apart.

## `TMarkupNode`

Covers the node tree between the markup text and the records.
The sample parses to a tree that formats back to the same bytes.
So the text cannot drift while the tree changes hands between Core and the adapter.
A node carries its line and its attributes.
Text with no root is refused, and a control character is dropped on write.

### `TMarkupNodeFind`

Answers the first descendant with the name, in document order, or nothing.

## `TMarkupParsing`

Covers the markup reader.
It checks what a well-formed file yields and what a strange one reports.
An unknown element or attribute is an omission with its line, never an error.
Malformed text or a foreign root is refused outright.
The refusal carries the markup reason, so the import stops before reading any entry.
The state attribute reads a value as unknown while an empty element reads as unspecified.
The two differ on a form, so the reader must not merge them.

The sample is the reference shape, so a field the reader drops shows first in the full-entry read.
An element the reader does not know is skipped, and the omission names it with its line.
The entry around it still reads, so one stray tag does not cost the import.
A collocation never nests, so a `meaning` inside one is skipped with its subtree.
Only `unknown` is a state.
Any other state value is named in the omissions and the text is read as specified.
The entry line is checked here too, since the import puts it on its own omissions.

A transcription with a blank scheme is skipped and reported at its line.
The save would refuse the whole file.
A second transcription whose trimmed scheme repeats the first is skipped and reported.
The first row stands, so the file order decides which text is kept.
A unit word the reader does not know leaves the unit unchosen and is reported at its line.
The user must see that the word was dropped.
An id attribute is reported as an omission with its line and is never read.
Row keys belong to the store, so a file cannot choose them.

The reader refuses nesting that is too deep.
The nesting sits inside a leaf, where no parser of ours recurses, but reading the leaf's text does.
Two hundred thousand levels would overflow the stack and end the process were the depth not refused first.
The ceiling counts the root.
A leaf on the third level may nest ceiling minus three deep and still read.
Twice the ceiling in bad attributes reports the ceiling and one `...` row.

## Stamp

Commit: `c2f335c331fe`
- `TMarkupFormatting.cs` `53ff55fe6a447494`
- `TMarkupNode.cs` `77beb2a7bb71e7e2`
- `TMarkupParsing.cs` `c6b7e95715b1a546`
- `TMarkupSample.cs` `12e7ebfb6ea226c1`
