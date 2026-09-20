# LMarkupFile.cs

## `public sealed class LMarkupFile : LMarkupVault`

The adapter behind the markup port, reading and writing `.llx` files on the file system.
It owns the XML side of the format, so Core holds the grammar and never names a parser.
The two static conversions are public so a test can hand text in and read text out without a file.

## `public const long LMarkupFileCeiling`

The largest file the import reads, sixty-four megabytes.
A file past it is refused before a byte is read, since markup that large is never an entry file.

## `public LMarkupNode LMarkupRead(string path)`

The element tree of the file at `path`, decoded as UTF-8 with any byte-order mark honoured.
A file past the ceiling raises the markup refusal.

## `public void LMarkupSave(string path, LMarkupNode root)`

Writes the tree under `root` to `path` as UTF-8 without a byte-order mark, replacing whatever was there.

## `public static LMarkupNode LMarkupFileParse(string text)`

Turns markup text into its element tree, line numbers kept on every node.
Malformed XML, a missing root or nesting past `LMarkup.LMarkupDepthCeiling` refuses with `LRefusal.LRefusalMarkup`.

## `public static string LMarkupFileFormat(LMarkupNode root)`

Writes the tree as indented markup text with no XML declaration.
Line ends are `\n` and the indent is two spaces, so the text is the same on every platform.

## `private static void LMarkupFileValidate(string text)`

Refuses with `LRefusal.LRefusalMarkup` when any element sits deeper than the depth ceiling, the root counted as one.
Every parser recurses on nested elements, and so does reading an element's text.
A file nested a hundred thousand deep would end the process, since a stack overflow cannot be caught.
The text is streamed rather than read as a tree, so the check stops at the first node too deep.
The tree reader itself survives deep nesting but takes minutes over it, which is why the stream runs first.
A legitimate file is read twice, and the stream pass is the cheaper of the two.

## `private static LMarkupNode LMarkupFileRead(XElement element)`

One element and everything under it as nodes, attributes and children in file order.
The text is the element's whole text content, as the parser reads it.
So a branch carries its leaves' text joined.

## `private static XElement LMarkupFileCreate(LMarkupNode node)`

One node and everything under it as elements, text passed through the normaliser.
A branch writes its children alone, since its text is only theirs read together.

## `private static string LMarkupFileNormalize(string text)`

The text with every character XML 1.0 cannot carry removed.
A control character pasted into a note would otherwise make the writer throw and the export fail.
The input is returned as is when nothing has to go, so the common case allocates nothing.
