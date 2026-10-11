# LPortraitFile.cs
Hash: `527bf6a2a0892d0c`

## `public sealed class LPortraitFile : LPortraitVault`

The adapter behind the portrait port, writing a Portrait as an HTML, Markdown, or Word file.
It is built over the theme the rendered page wears.
The engine therefore names neither the theme loader nor a writer.

## `public LPortraitFile(LTheme theme)`

Takes the theme once, so no export re-reads the embedded resource.

## `public void LPortraitSave(LPortraitPage page, LPortraitMedium format, string path)`

Writes `page` to `path` as the sheet, the outline, or the folio, by `format`.
Markup and PDF are refused, since the engine formats the one and the press prints the other.

## `public string LPortraitSheetFormat(LPortraitPage page)`

The HTML sheet of `page` wearing the current theme, which the press prints or saves as PDF.

## `public void LPortraitSave(LLiveryPage livery, LPortraitPage portrait, LPortraitLabel label, Func<string, string> lookup, string path)`

Writes the entry sheet to `path` as an HTML file.
Only HTML takes this road, since PDF goes through the press and the other formats keep the portrait alone.

## `public string LPortraitSheetFormat(LLiveryPage livery, LPortraitPage portrait, LPortraitLabel label, Func<string, string> lookup)`

The entry sheet of `LSheetEntry.LSheetEntryFormat` wearing the current theme.
The HTML file, the PDF and the print all read this one page.
