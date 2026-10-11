# LPortraitVault.cs
Hash: `cdf6145db9d1726f`

## `public interface LPortraitVault`

The port for writing a Portrait out of the program as a document.
`LPortraitFile` in Infrastructure is its adapter, which loads the theme the rendered page wears.
The engine composes the page and never learns how a format is rendered or written.

## `void LPortraitSave(LPortraitPage page, LPortraitMedium format, string path);`

Writes `page` to `path` in `format`.
Markup and PDF are not written here, since the engine formats the one and the press prints the other.

## `string LPortraitSheetFormat(LPortraitPage page);`

The HTML page of `page` wearing the current theme, which the press prints or saves as PDF.

## `void LPortraitSave(LLiveryPage livery, LPortraitPage portrait, LPortraitLabel label, Func<string, string> lookup, string path);`

Writes an entry's HTML export to `path` through the entry sheet writer.
It takes the Livery page beside the portrait page, so the file can show the entry as view mode does.
`lookup` words the labels the Livery page carries as keys.

## `string LPortraitSheetFormat(LLiveryPage livery, LPortraitPage portrait, LPortraitLabel label, Func<string, string> lookup);`

The entry sheet wearing the current theme, which the press prints or saves as PDF.
It is the same page the HTML export writes.
