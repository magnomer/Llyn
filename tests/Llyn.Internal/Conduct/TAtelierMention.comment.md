# TAtelierMention.cs

## `public sealed class TAtelierMention`

Covers the mention gates over fake ports that answer with the real span rules.
A linked Mention divides the text into three pieces, and only the linked one is marked.
An unlinked chip takes the silent label the driver hands in.
A selection and a caret convert between code points and UTF-16 units both ways.
A draft's Mentions resolve into stored marks, and no draft list reads none.

## `private static CAtelier TAtelierMentionCreate(LEngine engine, IReadOnlyList<LMentionLabel> labels)`

Builds the atelier whose resolve answers `labels` and whose span calls run the real span rules.
