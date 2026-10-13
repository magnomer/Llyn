# Markup

This folder holds the file-system adapter behind the markup port.
It owns the XML side of the .llx format, so Core holds the grammar and never names a parser.

## `LMarkupFile`

Reads and writes .llx files on the file system.
The two static conversions are public so a test can hand text in and read text out without a file.

## `LMarkupFileCeiling`

The largest file the import reads, sixty-four megabytes.
A file past it is refused before a byte is read, since markup that large is never an entry file.
The refusal is the markup refusal.

## `LMarkupRead`

Returns the element tree of a file, decoded as UTF-8 with any byte-order mark honoured.

## `LMarkupSave`

Writes the tree as UTF-8 without a byte-order mark, replacing whatever was there.

## `LMarkupFileParse`

Turns markup text into its element tree, with line numbers kept on every node.
Malformed XML, a missing root or nesting past `LMarkup.LMarkupDepthCeiling` refuses with `LRefusal.LRefusalMarkup`.

## `LMarkupFileFormat`

Writes the tree as indented markup text with no XML declaration.
Line ends are \n and the indent is two spaces, so the text is the same on every platform.

## `LMarkupFileValidate`

Refuses with `LRefusal.LRefusalMarkup` when any element sits deeper than the depth ceiling, the root counted as one.
Every parser recurses on nested elements, and so does reading an element's text.
A file nested a hundred thousand deep would end the process, since a stack overflow cannot be caught.
The text is streamed rather than read as a tree, so the check stops at the first node too deep.
The tree reader survives deep nesting but takes minutes over it, which is why the stream runs first.
A legitimate file is read twice, and the stream pass is the cheaper of the two.

## `LMarkupFileRead`

Turns one element and everything under it into nodes, with attributes and children in file order.
The text is the element's whole text content, as the parser reads it.
So a branch carries its leaves' text joined.

## `LMarkupFileCreate`

Turns one node and everything under it into elements, with text passed through the normaliser.
A branch writes its children alone, since its text is only theirs read together.

## `LMarkupFileNormalize`

Removes every character XML 1.0 cannot carry.
A control character pasted into a note would otherwise make the writer throw and the export fail.
The input is returned as is when nothing has to go, so the common case allocates nothing.

## Stamp

Commit: `c2f335c331fe`
- `LMarkupFile.cs` `7e52d86bc0c08ac2`
