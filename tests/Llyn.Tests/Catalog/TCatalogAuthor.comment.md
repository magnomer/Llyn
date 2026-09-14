# TCatalogAuthor.cs

## `public sealed class TCatalogAuthor`

Covers the Author catalog and the oeuvre read under one Author.
It covers the counts each row carries: how many Sources credit the Author and how many places cite those.
It covers name order, the two count orders with the busiest first, and the reversed name order.
It covers the query, which reads the name alone because a name is all an Author carries.
It covers the oeuvre of a chosen Author, of nobody, and of no choice at all.
It covers narrowing the oeuvre by a hidden kind and by typed text.
Every workspace starts with the Source titled Unknown, so an uncredited listing carries it among the rows the test made.

## `private static LReference TCatalogWorkCreate(LEngine engine, string title, LReferenceKind kind)`

Makes one titled Source of one kind through the engine.

## `private static void TCatalogCitationCreate(LEngine engine, string text, long referenceId)`

Makes one Example citing the Source, so the Source and its Authors count one place more.
