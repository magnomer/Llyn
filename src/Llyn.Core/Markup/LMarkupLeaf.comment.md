# LMarkupLeaf.cs

## `public static class LMarkupLeaf`

Writes one leaf element of a markup document.

## `public static void LMarkupLeafAppend(StringBuilder text, int depth, string name, LStateValue value)`

The three stored states map onto the three shapes the reader distinguishes.
An unset value writes no element, an unreadable one writes an empty element, and a set one writes its text.
Taking a state rather than a string lets a plain field and a stateful one share this method.

## `public static void LMarkupLeafAppend(StringBuilder text, int depth, string name, LStateValue value, string mark, string? marked)`

The same element, carrying one attribute.
`<meaning lang="English">` and `<audio source="...">` are the two the format writes this way.
An attribute with nothing to say is left off entirely rather than written empty.
An empty attribute would claim the value is unreadable, which neither of these two can be.

## `public static string LMarkupLeafNormalize(string value, string name)`

The reader ends an element at the first matching closing tag, and reads text raw.
So only that exact closing tag is dangerous inside the text, and only it is dropped.
Escaping instead would corrupt the value, because the reader unescapes nothing.
