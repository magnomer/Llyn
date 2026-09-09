# LMarkupCard.cs

## `public static class LMarkupCard`

Writes one meaning or collocation block.

## `public static void LMarkupCardAppend(StringBuilder text, string block, LCardDraft card)`

The block name is passed in because a meaning and a collocation differ only by their tag.
The element order follows the reader's own switch, so a written file reads back in one pass.
A video writes only its location, since the reader takes no span.
