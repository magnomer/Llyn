# LFolioLine.cs

## `public static class LFolioLine`

Writes the paragraph shapes a Word body is made of.

## `public static string LFolioLineFormat(string? text)`

A line break and a tab cannot ride inside a text run.
Each closes the run and opens a new one.
A carriage return is dropped, because Word takes the break from the element rather than the character.

## `public static void LFolioLineAppend(StringBuilder body, string style, string text)`

One styled paragraph, which is most of the document.

## `public static void LFolioLineAppend(StringBuilder body, string style, string mark, string text)`

A bold lead followed by plain text, in one paragraph.
The card number, the example bullet and the video marker all take this shape.

## `public static void LFolioLineDraw(StringBuilder body, int place, long width, long height)`

An inline picture, sized in the units the format measures documents in.
The relationship id is derived from the plate's place.
So the body and the relationships agree without being passed a map.
