# TMarkupNode.cs

## `public sealed class TMarkupNode`

Covers the node tree between the markup text and the records.
The sample with one element of every kind parses to a tree that formats back to the same bytes.
So the text cannot drift while the tree changes hands between Core and the adapter.
A node carries its line and its attributes.
Text with no root is refused, and a control character is dropped on write.

## `private static LMarkupNode? TMarkupNodeFind(LMarkupNode node, string name)`

The first descendant of `node` with the name, in document order, or nothing.
