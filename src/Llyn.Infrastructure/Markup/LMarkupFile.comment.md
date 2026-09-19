# LMarkupFile.cs

## `public sealed class LMarkupFile : LMarkupVault`

The adapter behind the markup port, reading and writing `.llx` files on the file system.
The engine parses and formats the text and never opens a file itself.

## `public const long LMarkupFileCeiling`

The largest file the import reads, sixty-four megabytes.
A file past it is refused before a byte is read, since markup that large is never an entry file.

## `public string LMarkupRead(string path)`

The whole text of the file at `path`, decoded as UTF-8 with any byte-order mark honoured.
A file past the ceiling raises the markup refusal.

## `public void LMarkupSave(string path, string text)`

Writes `text` to `path` as UTF-8 without a byte-order mark, replacing whatever was there.
