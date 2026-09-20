using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPortraitPage(
    string LPortraitPageTitle,
    string LPortraitPageLanguage,
    IReadOnlyList<string> LPortraitPageChip,
    IReadOnlyList<LPortraitSection> LPortraitPageSection,
    bool LPortraitPageFavorite = false,
    IReadOnlyList<LPortraitLine>? LPortraitPageLine = null)
{
    public IReadOnlyList<LPortraitLine> LPortraitPageLine { get; init; } = LPortraitPageLine ?? [];
}
