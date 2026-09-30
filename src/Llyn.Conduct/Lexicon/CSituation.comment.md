# CSituation.cs

## `public sealed record CSituation(`

One Situation ready for the repertoire's read page, the vignette.
Every field arrives worded and every row arrives chosen, so the driver only looks keys up and paints.

**Parameters**

- `CSituationTitle`: the title, worded untitled and muted while it was never written.
- `CSituationKind`: the kind, with no wording while it was never written, so its chip hides.
- `CSituationDescription`: the description, with no wording while it was never written, so its section hides.
- `CSituationMarkdown`: the description's text as Markdown blocks, which the driver draws while the description has no key.
- `CSituationImage`: the picture rows whose location is filled, in the order the user keeps them.
- `CSituationVideo`: the video rows whose location is filled, in the order the user keeps them.
