using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPortraitPage(
    string LPortraitPageTitle,
    string LPortraitPageLanguage,
    IReadOnlyList<string> LPortraitPageChip,
    IReadOnlyList<LPortraitSection> LPortraitPageSection);
