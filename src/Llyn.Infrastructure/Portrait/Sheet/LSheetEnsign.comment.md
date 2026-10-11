# LSheetEnsign.cs
Hash: `27a403c779ddd2cc`

## `public static class LSheetEnsign`

The flags of an exported entry page, each embedded once as a style rule.
A flag the page shows many times then costs its bytes only once.
The body names a flag by its class, which `LSheetEnsignRead` answers.

## `public static IReadOnlyList<string> LSheetEnsignScan(LLiveryPage page)`

The distinct flag paths the page can show, in the order their classes are numbered.
The banner of the entry's own language comes first, then the other language banners, then the variety ensigns.
A path that `LPortraitAsset.LPortraitAssetLoad` cannot read is left out, so no class points at a missing picture.

## `public static string LSheetEnsignFormat(IReadOnlyList<string> paths)`

One `.ensign-N` rule per path, carrying the flag as a background data address.
`N` is the path's place in `paths`, the number `LSheetEnsignRead` answers.
A path that no longer loads gets no rule, but keeps its number.

## `public static string LSheetEnsignRead(IReadOnlyList<string> paths, string? path)`

The class that shows `path`, or empty when the scan left it out.
