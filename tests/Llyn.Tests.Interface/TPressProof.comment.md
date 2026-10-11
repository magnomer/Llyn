# TPressProof.cs
Hash: `a7d147e6f63d54d9`

## `internal sealed class TPressProof(LPressPaper paper) : LPressProof`

A proof of three black pages, all on the one paper it was handed.
`TPressBoard` answers it, so a gate draws pages without a browser or a PDF reader.

## `internal bool TPressProofDisposed`

Whether the proof was disposed, so a test sees a superseded proof released.

## `internal TaskCompletionSource? TPressProofHold`

A gate that page draws wait on until the test releases it.
Null by default, so every page completes at once.
So a test can replace the proof while one of its pages is drawing.

## `public int LPressProofCount`

Always three, so a range can include some pages and leave others out.

## `public LPressPaper LPressProofRead(int page)`

Answers the one paper for every page, already turned by the orientation.

## `public async Task<LPressImage> LPressProofDraw(int page, int resolution)`

Waits for any hold, then answers a PNG of the paper at `resolution` dots per inch.
So a test reads the drawn size from the PNG header.

## `public void Dispose()`

Marks the proof as disposed and releases nothing, since it holds no resource.

## `private static byte[] TPressProofCreate(int across, int down)`

Builds a valid one-bit gray PNG `across` by `down` pixels, every pixel black.
It needs no drawing library, so the fake stays in plain .NET.

## `private static void TPressProofAdd(Stream png, string kind, byte[] data)`

Writes one PNG chunk of `kind` with its length and its CRC-32 check.
