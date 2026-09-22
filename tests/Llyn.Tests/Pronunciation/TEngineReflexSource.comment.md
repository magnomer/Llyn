# TEngineReflexSource.cs

## `public sealed class TEngineReflexSource`

Covers the page reading of one reflex rule over stubbed pages, split, gloss and dedupe included.
A Southern Min page with `split` parts paired readings and romanizations into rows.
Its gloss pattern parts a source note from a parenthesized meaning and leaves a bare gloss's meaning empty.
A dialect rule with `split` pairs each reading with its romanization.
Its gloss is looked up under the match through the gloss pattern, stopping at the `until` line.
A page listing one reading twice yields one row.
A match capturing a `note` group hands that text as the note without any gloss pattern.
A historical spelling equal to the current one folds into the marked row.
A different historical or ancient spelling is its own unmarked row labelled by the note.
A `main` group nested in the note marks the new-style row over the first.
A lone unlabelled row is still marked as the first.
