# LMarkupVault.cs

## `public interface LMarkupVault`

The port for markup files, the plain-text form entries travel in.
`LMarkupFile` in Infrastructure is its adapter over the file system.
The engine parses and formats markup and never opens a file itself.

## `string LMarkupRead(string path);`

The whole text of the file at `path`.
A file past the markup ceiling is refused before it is read.

## `void LMarkupSave(string path, string text);`

Writes `text` to `path`, replacing whatever was there.
