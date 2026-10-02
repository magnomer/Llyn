# TPortraitOutline.cs
Hash: `cc48a5b4e8ecdc12`

## `public sealed class TPortraitOutline`

Covers the Markdown rendering.

## `public void OutlineFormat_FullPortrait_KeepsTheDisplayReadingOrder()`

Markdown carries no theme, so order is the only fidelity it has.

## `public void OutlineFormat_MarkdownInFieldText_EscapesEveryControlCharacter()`

An unescaped angle bracket in field text would silently restructure the document.
