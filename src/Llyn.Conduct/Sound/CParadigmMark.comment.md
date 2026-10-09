# CParadigmMark.cs
Hash: `354a7f2f7eb052cd`

## `public sealed record CParadigmMark(int CParadigmMarkOffset, int CParadigmMarkLength)`

One marked range of a form, as the paradigm box paints it.
The range lies over the form's text, so the driver never counts letters itself.

**Parameters**

- `CParadigmMarkOffset`: the starting index in the form's UTF-16 text.
- `CParadigmMarkLength`: the marked length in UTF-16 code units.

## `internal static CParadigmMark CParadigmMarkCreate(LInflectionMark mark)`

Copies the two numbers of Core's mark.
It adds no rule.
