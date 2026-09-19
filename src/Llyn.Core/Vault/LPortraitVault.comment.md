# LPortraitVault.cs

## `public interface LPortraitVault`

The port for writing a Portrait out of the program as a document.
`LPortraitFile` in Infrastructure is its adapter, which loads the theme the rendered page wears.
The engine composes the page and never learns how a format is rendered or written.

## `void LPortraitSave(LPortraitPage page, LPortraitFormat format, string path);`

Writes `page` to `path` in `format`.
Markup and PDF are not written here, since the engine formats the one and the press prints the other.

## `string LPortraitSheetFormat(LPortraitPage page);`

The HTML page of `page` wearing the current theme, which the press prints or saves as PDF.
