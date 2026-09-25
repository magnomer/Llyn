# LPortraitFile.cs

## `public sealed class LPortraitFile : LPortraitVault`

The adapter behind the portrait port, writing a Portrait as an HTML, Markdown, or Word file.
It is built over the theme the rendered page wears.
The engine therefore names neither the theme loader nor a writer.

## `public LPortraitFile(LTheme theme)`

Takes the theme once, so no export re-reads the embedded resource.

## `public void LPortraitSave(LPortraitPage page, LPortraitFormat format, string path)`

Writes `page` to `path` as the sheet, the outline, or the folio, by `format`.
Markup and PDF are refused, since the engine formats the one and the press prints the other.

## `public string LPortraitSheetFormat(LPortraitPage page)`

The HTML sheet of `page` wearing the current theme, which the press prints or saves as PDF.
