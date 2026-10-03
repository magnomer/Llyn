# TMarkupParsing.cs
Hash: `ebc4a716a4fa2bcf`

## `public sealed class TMarkupParsing`

Covers the markup reader.
It checks what a well-formed file yields and what a strange one reports.
An unknown element or attribute is an omission with its line, never an error.
Malformed text or a foreign root is refused outright.
The state attribute reads a value as unknown while an empty element reads as unspecified.

## Inline notes

### `public void MarkupParse_FullEntry_ReadsEveryField()`

A file with every element yields one entry with each field in its place and no omission.
The sample is the reference shape, so a field the reader drops shows here first.

### `public void MarkupParse_UnknownElement_ReportsOmission()`

An element the reader does not know is skipped, and the omission names it with its line.
The entry around it still reads, so one stray tag does not cost the import.

### `public void MarkupParse_IdAttribute_ReportsOmission()`

An id attribute is reported as an omission with its line and is never read.
Row keys belong to the store, so a file cannot choose them.

### `public void MarkupParse_MalformedText_Refuses(string text)`

Text that is not well-formed, is rooted in a foreign element or is not markup is refused outright.
The refusal carries the markup reason, so the import stops before reading any entry.

### `public void MarkupParse_UnknownState_ReadsUnknown()`

A state attribute of unknown reads as an unknown value, and an empty element reads as unspecified.
The two differ on a form, so the reader must not merge them.

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

The ceiling counts the root.
A leaf on the third level may nest ceiling minus three deep and still read.

### `public void MarkupParse_FloodOfAttributes_CapsOmissions()`

Twice the ceiling in bad attributes reports the ceiling and one `...` row.
