# TCatalogOrder.cs

## `public sealed class TCatalogOrder`

Covers the named set of orderings a browsing panel picks from.
It covers an ordering written out and read back, which is what a remembered choice has to survive.
It covers text the set does not know, which falls back rather than failing.
A stored choice can outlive the ordering it named, so the fallback is behaviour and not defence.
