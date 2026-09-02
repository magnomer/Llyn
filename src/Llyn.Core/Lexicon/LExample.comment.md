# LExample.cs

## `public sealed record LExample(`

One Example — the first independent shared entity in the lexicon. An Example is owned by nothing: no Entry, Meaning, or Collocation contains it. Any number of them *reference* it instead, and the order an Example appears in lives on each reference rather than here, so the same Example can sit first under one Entry and third under a Meaning. `LExampleId` is the identity — an opaque, program-generated stable id; `LExampleText` is display text and never identity, so two Examples with identical text remain distinct rows.

An Example owns its `LExampleTranslations` and references *at most one* Source through `LExampleSource`. That reference is a pointer, not ownership: clearing it or deleting the Example never touches the Source. Every field here that can stand empty carries `LStateValue`, so a field holding nothing says whether nothing was ever recorded or something was recorded that cannot be read back.

**Parameters**

- `LExampleId` — Opaque, program-generated stable id.
- `LExampleLanguage` — Language code the example text is written in.
- `LExampleText` — The example text and what is known about it; display text, never identity.
- `LExampleLocal` — Optional local-script rendering of the text; `null` when absent.
- `LExampleSource` — The single referenced Source: its id when one is cited, nothing recorded when none is, unreadable when the citation cannot be read back.
- `LExampleTranslations` — The Example's owned translations, in order.
