# CExample.cs

## `public sealed record CExample(`

One stored Example, as the corpus transcript and excerpt read it.

**Parameters**

- `CExampleLanguage`: the language the example is written in.
- `CExampleText`: the example's text, uncertain when it is unknown.
- `CExampleSource`: the reference the example cites, null when none.
- `CExampleGloss`: the translations of the example, in order.
- `CExampleMention`: the Mentions the transcript's mention line shows.
- `CExampleExcerpt`: the Mentions the excerpt links, empty unless the text reads soundly.
