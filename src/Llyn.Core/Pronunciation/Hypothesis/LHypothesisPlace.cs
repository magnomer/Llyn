using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LHypothesisPlace(
    string LHypothesisPlaceName,
    IReadOnlyList<string> LHypothesisPlaceInitials)
{
    public string LHypothesisPlaceName { get; init; } = LHypothesisPlaceName ?? string.Empty;

    public IReadOnlyList<string> LHypothesisPlaceInitials { get; init; } = LHypothesisPlaceInitials ?? [];
}
