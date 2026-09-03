# LExample.cs

## `public sealed record LExample(`

One Example — the first independent shared entity in the lexicon.
An Example is owned by nothing: no Entry, Meaning, or Collocation contains it.
Any number of them *reference* it instead.
The order an Example appears in lives on each reference rather than here.
So the same Example can sit first under one Entry and third under a Meaning.
`LExampleId` is the identity, an opaque and program-generated stable id.
`LExampleText` is display text and never identity.
Two Examples with identical text remain distinct rows.

An Example owns its `LExampleRenditions` and references *at most one* Source through `LExampleSource`.
That reference is a pointer, not ownership: clearing it or deleting the Example never touches the Source.
Every field here that can stand empty carries `LStateValue`.
A field holding nothing says whether nothing was ever recorded.
It says instead when something was recorded that cannot be read back.

**Parameters**

- `LExampleId` — Opaque, program-generated stable id.
- `LExampleLanguage` — Language code the example text is written in.
- `LExampleText` — The example text and what is known about it, display text and never identity.
- `LExampleLocal` — Optional local-script rendering of the text, and `null` when absent.
- `LExampleSource` — The single referenced Source, by its id when one is cited.
  Nothing was recorded when none is cited.
  It is unreadable when the citation cannot be read back.
- `LExampleRenditions` — The Example's owned renditions, in order.
