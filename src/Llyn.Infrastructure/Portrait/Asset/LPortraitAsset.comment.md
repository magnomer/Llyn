# LPortraitAsset.cs
Hash: `be37daa2bcbf516e`

## `public sealed record LPortraitAsset(byte[] LPortraitAssetData, string LPortraitAssetMedia, string LPortraitAssetSuffix)`

One picture read off disk or out of a data address, ready to embed.

**Parameters**

- `LPortraitAssetData`: the file's bytes.
- `LPortraitAssetMedia`: the media type, taken from the extension or the data address.
- `LPortraitAssetSuffix`: the extension, kept for the package part name.

## `private const long LPortraitAssetCeiling`

The largest file read, 24 megabytes.
A larger picture would bloat the document past what a viewer opens comfortably.

## `public string LPortraitAssetAddress`

The bytes as an address a page can carry without a second file.

## `public static LPortraitAsset? LPortraitAssetLoad(string location)`

Only a local file is read, because an export makes no network request.
A remote picture is left to the page, which may still fetch it when opened.
An oversized file is refused, since one picture must not make a document unopenable.
A missing or unknown file yields nothing, and the writer falls back to its address.
A file whose suffix names no known picture type yields nothing as well.
Guessing JPEG for it would embed bytes no viewer can draw.
A data address is decoded in place, which is how a stored character form reaches a document.

## `private static LPortraitAsset? LPortraitAssetParse(string address)`

The media type and bytes of a base64 data address, or nothing when it is malformed.
The suffix is chosen from the media type, because a package part needs an extension.
An unknown media type takes `.jpg`, since the bytes are kept as given.
