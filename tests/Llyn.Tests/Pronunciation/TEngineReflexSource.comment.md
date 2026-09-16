# TEngineReflexSource.cs

## `public sealed class TEngineReflexSource`

Covers the page reading of one reflex rule over stubbed pages, split, remark and dedupe included.
A dialect rule with `split` parts a paired reading into rows, each paired with its own note.
Its remark is looked up under the match through the remark pattern, stopping at the `until` line.
A page listing one reading twice yields one row.
A match capturing a `remark` group hands that text as the remark without any remark pattern.
A historical spelling equal to the current one folds into the marked row.
A different historical or ancient spelling is its own unmarked row labelled by the remark.
