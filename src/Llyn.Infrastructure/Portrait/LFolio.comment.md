# LFolio.cs

## `public static class LFolio`

Writes a page likeness as a Word document.
The package is built directly rather than through a document library, so the project takes no new dependency.
Only the small part of the format Word needs to open a file is written.

## `public static void LFolioSave(LPortraitPage page, LTheme theme, string path)`

The body is built first, because building it is what discovers which pictures are embedded.
The content types and the relationships both depend on that list, so they are written after.

## `private static void LFolioTextSave(ZipArchive package, string name, string text)`

Word rejects a byte order mark in a package part, so the encoding writes none.
