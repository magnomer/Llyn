# LPortraitAsset.cs

## `public sealed record LPortraitAsset`

One picture read off disk, ready to embed.

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
