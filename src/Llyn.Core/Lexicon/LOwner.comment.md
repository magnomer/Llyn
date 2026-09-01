# LOwner.cs

## `public enum LOwner`

Which kind of stored row an id names when a seam takes the referring side of a reference. An Example, a Tag and a Situation are independent data several kinds of row may point at, so a seam that reads, attaches or detaches one of them needs the caller to say which side the id it carries belongs to.

It exists because a name may carry three components after its prefix, so a seam cannot spell the owner into its own name — one method per owner side would read `…TagSenseAttach`, one component too many. Naming the side as a value keeps one seam per operation and makes the set of sides a thing the compiler checks rather than a spelling convention.

Not every seam accepts every side: a Tag hangs from a Meaning or a Collocation and from nothing else, and a seam handed a side its entity has no association for refuses the request rather than guessing at a table.

## `LOwnerEntry,`

The id names an Entry.

## `LOwnerSense,`

The id names a Meaning.

## `LOwnerCollocation,`

The id names a Collocation.

## `LOwnerExample,`

The id names an Example.

## `LOwnerReference,`

The id names a Reference.
