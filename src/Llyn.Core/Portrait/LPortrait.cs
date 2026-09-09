using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPortrait(
    string LPortraitHeadword,
    string LPortraitLanguage,
    string LPortraitPronunciation,
    IReadOnlyList<string> LPortraitSpeech,
    IReadOnlyList<LPortraitCard> LPortraitMeaning,
    IReadOnlyList<LPortraitCard> LPortraitCollocation,
    IReadOnlyList<LPortraitUsage> LPortraitIncoming,
    string LPortraitNote,
    bool LPortraitFavorite,
    LPortraitLabel LPortraitLabel);
