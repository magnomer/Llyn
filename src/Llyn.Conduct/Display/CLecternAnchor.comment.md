# CLecternAnchor.cs
Hash: `654f4dc6482c4df1`

## `public sealed record CLecternAnchor(bool CLecternAnchorOffered, IReadOnlyDictionary<long, string> CLecternAnchorTexts)`

How reflex rows anchor to the fanqie rows, ready to show.
The reading view and the editor's reflex block both carry it.

**Parameters**

- `CLecternAnchorOffered`: whether the headword's fanqie rows let a reflex row anchor at all.
- `CLecternAnchorTexts`: the anchor text of each reflex row, keyed by the row id.
