# TWarrantFake.cs
Hash: `70fd35ae4c07f303`

## `public sealed class TWarrantFake : LWarrant`

A warrant that marks and reverses the token, so tests need no operating system protection.
The output is visibly not the plain token and is exactly undone by restoring.

## `private const string TWarrantFakeMark = "fake:";`

A fixed marker that lets restoring tell this fake's text from anything else, as the real adapter rejects foreign data.

## `public string LWarrantHide(string token)`

Refuses a null or blank token like the real adapter, so a test cannot hide nothing by accident.
Otherwise it returns the marker followed by the reversed token.

## `public string? LWarrantRestore(string hidden)`

Answers null for blank text or text lacking the marker, as the real adapter does for unrecoverable input.
Otherwise it returns the token reversed back.

## `private static string TWarrantFakeFormat(string text)`

Shares one reversal between hiding and restoring.
