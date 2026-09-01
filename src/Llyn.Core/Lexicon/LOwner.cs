namespace Llyn.Core;

/// <summary>
/// Which kind of stored row an id names when a seam takes the referring side of a reference. An
/// Example, a Tag and a Situation are independent data several kinds of row may point at, so a seam
/// that reads, attaches or detaches one of them needs the caller to say which side the id it carries
/// belongs to.
/// <para>
/// It exists because a name may carry three components after its prefix, so a seam cannot spell the
/// owner into its own name — one method per owner side would read <c>…TagSenseAttach</c>, one
/// component too many. Naming the side as a value keeps one seam per operation and makes the set of
/// sides a thing the compiler checks rather than a spelling convention.
/// </para>
/// <para>
/// Not every seam accepts every side: a Tag hangs from a Meaning or a Collocation and from nothing
/// else, and a seam handed a side its entity has no association for refuses the request rather than
/// guessing at a table.
/// </para>
/// </summary>
public enum LOwner
{
    /// <summary>The id names an Entry.</summary>
    LOwnerEntry,

    /// <summary>The id names a Meaning.</summary>
    LOwnerSense,

    /// <summary>The id names a Collocation.</summary>
    LOwnerCollocation,

    /// <summary>The id names an Example.</summary>
    LOwnerExample,

    /// <summary>The id names a Reference.</summary>
    LOwnerReference,
}
