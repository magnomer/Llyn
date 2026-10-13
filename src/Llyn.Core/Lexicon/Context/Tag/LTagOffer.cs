using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LTagOffer(
    string LTagOfferText,
    IReadOnlyList<LTagRow> LTagOfferRows,
    bool LTagOfferShown);
