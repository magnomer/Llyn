# LPortraitAsset.cs

## `public sealed record LPortraitAsset`

One picture read off disk or out of a data address, ready to embed.

**Parameters**

- `LPortraitAssetData` - the file's bytes.
- `LPortraitAssetMedia` - the media type, taken from the extension.
- `LPortraitAssetSuffix` - the extension, kept for the package part name.

## `public string LPortraitAssetAddress`

The bytes as an address a page can carry without a second file.

## `public static LPortraitAssetLoad(string location)`

Only a local file is read, because an export makes no network request.
A remote picture is left to the page, which may still fetch it when opened.
An oversized file is refused, since one picture must not make a document unopenable.
A missing or unknown file yields nothing, and the writer falls back to its address.
A data address is decoded in place, which is how a stored character form reaches a document.

## `private static LPortraitAsset? LPortraitAssetParse(string address)`

The media type and bytes of a base64 data address, or nothing when it is malformed.
The suffix is chosen from the media type, because a package part needs an extension.
