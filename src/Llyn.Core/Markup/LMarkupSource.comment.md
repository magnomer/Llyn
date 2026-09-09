# LMarkupSource.cs

## `public static class LMarkupSource`

Writes one source block with its credited authors.

## `public static void LMarkupSourceAppend(StringBuilder text, LMarkup.LMarkupReference source)`

The id is written as an attribute, because a citation names a source by that id alone.
Authors are written in credited order, which is the order the reader restores.
