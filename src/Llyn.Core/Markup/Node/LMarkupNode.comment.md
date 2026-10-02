# LMarkupNode.cs
Hash: `0772124e410ccc3b`

## `public sealed record LMarkupNode(`

One element of a markup file as a value, free of the parser that read it.
The reader walks a tree of these and the writer builds one, so Core never names XML.
`LMarkupFile` in Infrastructure turns text into the tree and the tree back into text.

**Parameters**

- `LMarkupNodeName` — The element name, plain lowercase English as the format contract says.
- `LMarkupNodeAttribute` — Every attribute the element carried.
- `LMarkupNodeChild` — The child elements in file order.
- `LMarkupNodeText` — The text content, empty for an element that holds only children.
- `LMarkupNodeLine` — The line the element started on, zero for a node the writer built.

## `public static LMarkupNode LMarkupNodeCreate(string name, string text = "")`

A leaf with no attributes and no children, the shape every text field takes.

## `public static LMarkupNode LMarkupNodeCreate(string name, IReadOnlyList<LMarkupNode> children)`

A branch with no attributes and no text of its own.
