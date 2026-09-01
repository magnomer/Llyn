namespace Llyn.Core;

/// <summary>
/// One Collocation owned by an entry: a stable-id node mirroring the Meaning construction, carrying an
/// <see cref="LCollocationExpression"/> — the phrase itself — beside the
/// <see cref="LCollocationMeaning"/> that explains it, which is the definition analogue of a Meaning.
/// The card has a field for each, so a collocation keeps both, beside the
/// <see cref="LCollocationTitle"/> the card is headed with. <see cref="LCollocationId"/> is the
/// identity — an opaque, program-generated stable id — and is the base the job08/job09 associations
/// target. Reordering collocation cards rewrites <see cref="LCollocationPosition"/> only; the id never
/// changes. A collocation's synonym is not text held here: it is an interlink, modelled by
/// <see cref="LCollocationSynonym"/>.
/// </summary>
/// <param name="LCollocationId">Opaque, program-generated stable id.</param>
/// <param name="LCollocationEntryId">Owning entry id.</param>
/// <param name="LCollocationPosition">Order among the entry's collocations.</param>
/// <param name="LCollocationTitle">Title typed on the Collocation card; <c>null</c> when none was typed.</param>
/// <param name="LCollocationExpression">The collocation expression text; empty when unset.</param>
/// <param name="LCollocationMeaning">What the expression means; empty when unset.</param>
public sealed record LCollocation(
    string LCollocationId,
    string LCollocationEntryId,
    int LCollocationPosition,
    string? LCollocationTitle,
    string? LCollocationExpression,
    string? LCollocationMeaning);
