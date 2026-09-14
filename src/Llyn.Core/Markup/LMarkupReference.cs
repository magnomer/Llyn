using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LMarkupReference(
    LStateValue LMarkupReferenceTitle,
    LStateValue LMarkupReferenceYear,
    LReferenceKind LMarkupReferenceKind,
    LStateValue LMarkupReferenceUrl,
    LStateValue LMarkupReferenceNote,
    IReadOnlyList<string>? LMarkupReferenceAuthor = null) : IEquatable<LMarkupReference>
{
    public LStateValue LMarkupReferenceTitle { get; init; } =
        LMarkupReferenceTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LMarkupReferenceYear { get; init; } =
        LMarkupReferenceYear ?? LStateValue.LStateValueUnspecified;

    public LStateValue LMarkupReferenceUrl { get; init; } =
        LMarkupReferenceUrl ?? LStateValue.LStateValueUnspecified;

    public LStateValue LMarkupReferenceNote { get; init; } =
        LMarkupReferenceNote ?? LStateValue.LStateValueUnspecified;

    public IReadOnlyList<string> LMarkupReferenceAuthor { get; init; } = LMarkupReferenceAuthor ?? [];

    public bool Equals(LMarkupReference? other)
    {
        return other is not null
            && LMarkupReferenceTitle.Equals(other.LMarkupReferenceTitle)
            && LMarkupReferenceYear.Equals(other.LMarkupReferenceYear)
            && LMarkupReferenceKind == other.LMarkupReferenceKind
            && LMarkupReferenceUrl.Equals(other.LMarkupReferenceUrl)
            && LMarkupReferenceNote.Equals(other.LMarkupReferenceNote)
            && LMarkupReferenceAuthor.SequenceEqual(other.LMarkupReferenceAuthor);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            LMarkupReferenceTitle,
            LMarkupReferenceYear,
            LMarkupReferenceKind,
            LMarkupReferenceUrl,
            LMarkupReferenceNote,
            LMarkupReferenceAuthor.Count);
    }
}
