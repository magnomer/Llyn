# LOwner.cs

## `public enum LOwner`

Which kind of stored row an id names when a seam takes the referring side of a reference.
An Example and a Situation are independent data several kinds of row may point at.
A Tag is text several kinds of row may carry.
An Example cites a Source of its own.
So a seam that reads, attaches or detaches one of them needs the caller to name a side.
The side says which kind of row the id it carries belongs to.

It exists because a name may carry three components after its prefix.
So a seam cannot spell the owner into its own name.
One method per owner side would read `…TagMeaningAttach`, one component too many.
Naming the side as a value keeps one seam per operation.
It makes the set of sides something the compiler checks rather than a spelling convention.

Not every seam accepts every side.
A Tag hangs from a Meaning or a Collocation and from nothing else.
A seam handed a side its entity has no association for refuses the request.
It never guesses at a table.

## `LOwnerEntry,`

The id names an Entry.

## `LOwnerMeaning,`

The id names a Meaning.

## `LOwnerCollocation,`

The id names a Collocation.

## `LOwnerExample,`

The id names an Example.

## `LOwnerSituation,`

The id names a Situation.

## `LOwnerReference,`

The id names a Reference.
