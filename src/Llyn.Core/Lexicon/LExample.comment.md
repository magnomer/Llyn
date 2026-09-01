# LExample.cs

## `public sealed record LExample(`

One Example — the first independent shared entity in the lexicon. An Example is owned by nothing: no Entry, Meaning, or Collocation contains it. Any number of them *reference* it instead, and the order an Example appears in lives on each reference rather than here, so the same Example can sit first under one Entry and third under a Meaning. `LExampleId` is the identity — an opaque, program-generated stable id; `LExampleText` is display text and never identity, so two Examples with identical text remain distinct rows.

An Example owns its `LExampleTranslations` and references *at most one* Source through `LExampleSourceId`. That reference is a pointer, not ownership: clearing it or deleting the Example never touches the Source.

**Parameters**

- `LExampleId` — Opaque, program-generated stable id.
- `LExampleLanguage` — Language code the example text is written in.
- `LExampleText` — The example text; display text, never identity.
- `LExampleLocal` — Optional local-script rendering of the text; `null` when absent.
- `LExampleSourceId` — Id of the single referenced Source, or `null` when the Example cites none.
- `LExampleTranslations` — The Example's owned translations, in order.
