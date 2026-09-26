# TPortraitOutline.cs

## `public sealed class TPortraitOutline`

Covers the Markdown rendering.

## `public void OutlineFormat_FullPortrait_KeepsTheDisplayReadingOrder()`

Markdown carries no theme, so order is the only fidelity it has.

## `public void OutlineFormat_MarkdownInFieldText_EscapesEveryControlCharacter()`

An unescaped hash or asterisk would silently restructure the document.
