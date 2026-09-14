# LMarkupMention.cs

## `public sealed record LMarkupMention(`

A word inside an example that stands for an entry, as a markup file carries it.
The target is named by headword and language, and a sense by its position path.

**Parameters**

- `LMarkupMentionOffset` — Where the span starts in the sentence text.
- `LMarkupMentionLength` — How many characters the span covers.
- `LMarkupMentionHeadword` — The headword of the entry the word stands for, empty for a span with no entry.
- `LMarkupMentionLanguage` — The language of that entry.
- `LMarkupMentionSense` — A one-based position path such as `1.2`, empty when no sense is named.
