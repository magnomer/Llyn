# TMarkupParse.cs

## `public sealed class TMarkupParse`

Covers the markup reader: what a well-formed file yields and what a strange one reports.
An unknown element or any attribute is an omission with its line, never an error.
Malformed text or a foreign root is refused outright.
The state attribute reads a value as unknown while an empty element reads as unspecified.

## Inline notes

### `public void MarkupParse_NestedCollocation_ReportsOmission()`

A collocation never nests, so a `meaning` inside one is skipped with its subtree.

### `public void MarkupParse_OtherStateValue_ReportsOmission()`

Only `unknown` is a state.
Any other value is named in the omissions and the text is read as specified.
The entry line is checked here too, since it is what the import puts on its own omissions.

### `public void MarkupParse_DeepLeafNesting_RefusesMarkup()`

The nesting sits inside a leaf, where no parser of ours recurses, but reading the leaf's text does.
Two hundred thousand levels would overflow the stack and end the process were the depth not refused first.

### `public void MarkupParse_NestingAtCeiling_Reads()`

The ceiling counts the root, so a leaf three below it may nest ceiling minus three deep and still read.

### `public void MarkupParse_FloodOfAttributes_CapsOmissions()`

Twice the ceiling in bad attributes reports the ceiling and one `...` row.
