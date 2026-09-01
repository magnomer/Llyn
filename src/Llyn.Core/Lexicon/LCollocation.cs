namespace Llyn.Core;

/// <summary>
/// One Collocation owned by an entry: a stable-id node mirroring the Meaning construction, except it
/// carries an <see cref="LCollocationExpression"/> instead of a definition (the UI counterpart of
/// <c>PSenseDefinition</c> is <c>PCollocationExpression</c>). <see cref="LCollocationId"/> is the
/// identity — an opaque, program-generated stable id — and is the base the job08/job09 associations
/// target. Reordering collocation cards rewrites <see cref="LCollocationPosition"/> only; the id never
/// changes. A collocation's synonym is not text held here: it is an interlink, modelled by
/// <see cref="LCollocationSynonym"/>.
/// </summary>
/// <param name="LCollocationId">Opaque, program-generated stable id.</param>
/// <param name="LCollocationEntryId">Owning entry id.</param>
/// <param name="LCollocationPosition">Order among the entry's collocations.</param>
/// <param name="LCollocationExpression">The collocation expression text; empty when unset.</param>
public sealed record LCollocation(
    string LCollocationId,
    string LCollocationEntryId,
    int LCollocationPosition,
    string? LCollocationExpression);
