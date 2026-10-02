# CMarkupOmission.cs
Hash: `54cc1b1f6e97dba1`

## `public sealed record CMarkupOmission(string CMarkupOmissionLine, string CMarkupOmissionText)`

One thing a markup import left behind, as the import report lists it.
Both fields arrive as ready text, so a driver only shows them.

**Parameters**

- `CMarkupOmissionLine`: the line of the file it sits on, written in the current culture, zero when unknown.
- `CMarkupOmissionText`: what was skipped.
