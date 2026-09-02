# LTag.cs

## `public sealed record LTag(`

One Tag — independent data owned by nothing. No Entry, Meaning, or Collocation contains a Tag; any number of Meanings and Collocations *reference* it instead, and the order a Tag appears in lives on each reference rather than here, so the same Tag can sit first under one Meaning and third under a Collocation. `LTagId` is the identity — an opaque, program-generated stable id; `LTagText` is display text and never identity, so editing the text leaves the id and every reference to it untouched, and two Tags reading alike remain distinct rows.

**Parameters**

- `LTagId` — Opaque, program-generated stable id.
- `LTagText` — The tag text and what is known about it; display text, never identity. A Tag that is there but unreadable is not a Tag that was never written.
