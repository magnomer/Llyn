# LMarkupExample.cs

## `public static class LMarkupExample`

Writes one example line, which is the only card element carrying attributes.

## `public static void LMarkupExampleAppend(StringBuilder text, LSentenceDraft sentence)`

The row is what a card holds, so the frame is read off it and the sentence off the Example it names.
A row naming no Example is not written, because this writer has no line for a frame alone.
An Example with no text is not written either, because it has nothing to cite or frame.
The citation, particle and dependence ride as attributes, exactly as the reader expects them.
