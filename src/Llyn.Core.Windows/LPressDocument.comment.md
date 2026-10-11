# LPressDocument.cs
Hash: `108ca38d745071a2`

## `internal sealed class LPressDocument : LPressProof`

One rendered PDF of the sheet, drawn to page images by the system PDF reader.
The preview shows these images, so it shows exactly what the browser printed.

## `private readonly PdfDocument _lPressDocumentPdf`

The loaded PDF the pages are read and drawn from.

## `private readonly Stream _lPressDocumentStream`

A seekable copy of the PDF the browser printed.
The reader reads from it while drawing, so it lives as long as the proof.

## `private LPressDocument(PdfDocument pdf, Stream stream)`

Reached only through `LPressDocumentOpen`, so no proof exists without a loaded PDF.

## `public int LPressProofCount`

The page count the PDF reports, known without drawing any page.

## `internal static async Task<LPressDocument> LPressDocumentOpen(Stream stream)`

Copies `stream` into memory, disposes it, and loads the PDF from the copy.
The browser's stream cannot seek, and the PDF reader needs one that can.
On any failure both streams are disposed and the exception rethrows, since no proof exists to dispose them later.

## `public LPressPaper LPressProofRead(int page)`

Answers the page's own size in inches, read from the PDF.
It is never computed from the setting, so rounding cannot make the view and the PDF differ.
The reader measures in units of a ninety-sixth of an inch.
Pages count from zero.

## `public async Task<LPressImage> LPressProofDraw(int page, int resolution)`

Draws one page as a PNG at `resolution` dots per inch and answers the bytes.
The page size in device-independent units is 96 per inch, so both sides scale by `resolution / 96`.
Each side keeps at least one pixel.
It may run off the UI thread, because the PDF reader has no thread affinity.
Pages count from zero.

## `public void Dispose()`

Releases the PDF copy the reader draws from.
