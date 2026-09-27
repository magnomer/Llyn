# CMarkupOmission.cs

## `public sealed record CMarkupOmission(int CMarkupOmissionLine, string CMarkupOmissionText)`

One thing a markup import left behind, as the import report lists it.

**Parameters**

- `CMarkupOmissionLine`: the line of the file it sits on, zero when unknown.
- `CMarkupOmissionText`: what was skipped.
