using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LMarkupCargo(
    IReadOnlyList<LMarkupEntry> LMarkupCargoEntry,
    IReadOnlyList<LMarkupOmission> LMarkupCargoOmission)
{
    public IReadOnlyList<LMarkupEntry> LMarkupCargoEntry { get; init; } = LMarkupCargoEntry ?? [];

    public IReadOnlyList<LMarkupOmission> LMarkupCargoOmission { get; init; } = LMarkupCargoOmission ?? [];
}
