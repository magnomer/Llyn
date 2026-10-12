using System;
using System.Collections.Generic;
using System.Linq;

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

    public static LPortraitPage LPortraitPageOpen(LPortraitPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        return page with
        {
            LPortraitPageSection = page.LPortraitPageSection.Select(LPortraitSection.LPortraitSectionOpen).ToList(),
        };
    }
}
