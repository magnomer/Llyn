# Asset

This folder holds what a Portrait export draws with, and the writer that saves it.
It reads pictures, icons and the theme, and sizes pictures without a decoder.
It stands apart from Folio and Sheet because those build documents and this supplies their inputs.

## `LPortraitAsset`

It is one picture read off disk or out of a data address, ready to embed.
The record keeps the bytes, the media type and the extension.
The extension is kept for the package part name.
The address property gives the bytes as an address a page can carry without a second file.

### `LPortraitAssetCeiling`

The largest file read is 24 megabytes.
A larger picture would bloat the document past what a viewer opens comfortably.

### `LPortraitAssetLoad`

Only a local file is read, because an export makes no network request.
A remote picture is left to the page, which may still fetch it when opened.
An oversized file is refused, since one picture must not make a document unopenable.
A missing or unknown file yields nothing, and the writer falls back to its address.
A file whose suffix names no known picture type yields nothing as well.
Guessing JPEG for it would embed bytes no viewer can draw.
A data address is decoded in place, which is how a stored character form reaches a document.

### `LPortraitAssetParse`

It returns the media type and bytes of a base64 data address, or nothing when malformed.
The suffix is chosen from the media type, because a package part needs an extension.
An unknown media type takes `.jpg`, since the bytes are kept as given.

## `LPortraitFile`

It is the adapter behind the portrait port, writing a Portrait as an HTML, Markdown or Word file.
It is built over the theme the rendered page wears.
The engine therefore names neither the theme loader nor a writer.
The theme is taken once, so no export re-reads the embedded resource.

### `LPortraitSave`

The portrait overload writes the sheet, the outline or the folio by format.
Markup and PDF are refused, since the engine formats the one and the press prints the other.
The entry overload writes the entry sheet as HTML only.
PDF goes through the press and the other formats keep the portrait alone.

### `LPortraitSheetFormat`

The HTML sheet wears the current theme, and the press prints or saves it as PDF.
The entry sheet comes from `LSheetEntry.LSheetEntryFormat`.
The HTML file, the PDF and the print all read this one page.

## `LPortraitFilm`

It recognizes a hosted video address.
A hosted video is the one kind whose poster frame can be named without decoding anything.
`LPortraitFilmRead` reads the location as an address, and Core's `LVideoFilmRead` names the film.
The video screen asks the same rule, so both accept the same address shapes.
A local file yields nothing, because no frame can be taken from it without a decoder.

## `LPortraitIcon`

It reads the icons an exported page draws out of the assembly rather than off disk.
An export then embeds them even when no icon folder stands beside the program.
The logical-name prefix is the one `Llyn.Infrastructure.csproj` gives each embedded icon.
`LPortraitIconRead` takes a name without its `.svg` suffix.
The icons are fixed at build, so an unknown name is a caller mistake and throws.

## `LPortraitSize`

It reads a picture's pixel size from its own header.
A Word picture must be sized when it is placed, and no decoder is available here.
Four container shapes are recognized from their opening bytes.
Anything else falls back to a plain widescreen size rather than failing the export.
A JPEG keeps its size in a frame marker somewhere after the start.
Markers are walked until a frame marker is met, and the scan stops at a malformed length.

## `LThemeLoader`

It reads an embedded theme file into the core's `LTheme` record.
The palettes of `themes/` are embedded here alone, so every palette in the program comes through this one read.
The read without a name loads `default`.
A missing or broken theme is not a failure worth stopping an export for.
Every colour then falls back to the value the record ships with.
The read with a name serves the console themes and hides no failure.
An unknown name throws `FileNotFoundException` and broken JSON throws `JsonException`.
The console start catches either and falls back to its own spare palette.
A blank name throws, since it can name no resource.

## Stamp

Commit: `92c4426f8506`
- `LPortraitAsset.cs` `be37daa2bcbf516e`
- `LPortraitFile.cs` `527bf6a2a0892d0c`
- `LPortraitFilm.cs` `5295c2a2ef3a8a94`
- `LPortraitIcon.cs` `a1df0e263a968016`
- `LPortraitSize.cs` `cfd128cc5638459c`
- `LThemeLoader.cs` `32889cb456aa998f`
