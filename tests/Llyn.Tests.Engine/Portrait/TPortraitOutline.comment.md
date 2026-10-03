# TPortraitOutline.cs
Hash: `728c6ba7f9b41947`

## `public sealed class TPortraitOutline`

Covers the Markdown rendering.

## `public void OutlineFormat_FullPortrait_KeepsTheDisplayReadingOrder()`

Markdown carries no theme, so order is the only fidelity it has.

## `public void OutlineFormat_MarkdownInFieldText_EscapesEveryControlCharacter()`

An unescaped angle bracket, hash or asterisk in field text would silently restructure the document.
Every character the formatter escapes is checked in the title, language and chip, and through the normalizer directly.
