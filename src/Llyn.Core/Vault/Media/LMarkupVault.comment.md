# LMarkupVault.cs

## `public interface LMarkupVault`

The port for markup files, the plain-text form entries travel in.
`LMarkupFile` in Infrastructure is its adapter over the file system and the XML parser.
The engine reads and writes the `LMarkupNode` tree and never sees the text.

## `LMarkupNode LMarkupRead(string path);`

The element tree of the file at `path`.
A file past the markup ceiling, malformed XML or nesting too deep refuses with `LRefusal.LRefusalMarkup`.

## `void LMarkupSave(string path, LMarkupNode root);`

Writes the tree under `root` to `path` as markup text, replacing whatever was there.
