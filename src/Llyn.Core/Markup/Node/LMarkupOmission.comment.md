# LMarkupOmission.cs
Hash: `40e39dad503436c1`

## `public sealed record LMarkupOmission(`

One thing a markup read or import left behind, named so the reader can find it in the file.
The parser reports unknown elements and attributes here, and the import later adds what it could not resolve.

**Parameters**

- `LMarkupOmissionLine` — The line of the file the omission sits on, zero when unknown.
- `LMarkupOmissionText` — What was skipped, such as `<colour>` or `id="42"`.
