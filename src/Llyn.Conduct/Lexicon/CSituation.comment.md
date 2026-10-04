# CSituation.cs
Hash: `e5d67ef144ab7497`

## `public sealed record CSituation(CStateWording CSituationTitle, CStateWording CSituationKind, CStateWording CSituationDescription, IReadOnlyList<CMarkdownBlock> CSituationMarkdown, IReadOnlyList<CImageDraft> CSituationImage, IReadOnlyList<CVideoDraft> CSituationVideo)`

One Situation ready for the repertoire's read page, the vignette.
Every field arrives worded and every row arrives chosen, so the driver only looks keys up and paints.

**Parameters**

- `CSituationTitle`: the title, worded untitled and muted while it was never written.
- `CSituationKind`: the kind, with no wording while it was never written, so its chip hides.
- `CSituationDescription`: the description, with no wording while it was never written, so its section hides.
- `CSituationMarkdown`: the description's text as Markdown blocks, which the driver draws while the description has no key.
- `CSituationImage`: the picture rows whose location is filled, in the order the user keeps them.
- `CSituationVideo`: the video rows whose location is filled, in the order the user keeps them.
